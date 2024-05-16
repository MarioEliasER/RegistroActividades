using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegistroActividades.Models.DTOs;
using RegistroActividades.Models.Entities;
using RegistroActividades.Models.Validators;
using RegistroActividades.Repositories;

namespace RegistroActividades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadesController : ControllerBase
    {
        private readonly ActividadesRepository repository;

        public ActividadesController(ActividadesRepository repository)
        {
            this.repository = repository;
        }

        private int ObtenerIdDepartamento()
        {
            if (User.Identity.IsAuthenticated)
            {
                var departamento = int.Parse(User.FindFirst("IdDepartamento")?.Value);
                return departamento;
            }
            else
            {
                throw new UnauthorizedAccessException("El usuario no está autenticado.");
            }
        }

        [HttpPost]
        public IActionResult Post(ActividadDTO dto)
        {
            ActividadValidator validator = new ActividadValidator();
            var departamento = ObtenerIdDepartamento();
            var resultado = validator.Validate(dto);
            if (resultado.IsValid)
            {
                Actividades actividades = new Actividades()
                {
                    Id = 0,
                    Titulo = dto.Titulo,
                    Descripcion = dto.Descripcion,
                    FechaCreacion = dto.FechaCreacion,
                    FechaActualizacion = dto.FechaActualizacion,
                    Estado = dto.Estado,
                    FechaRealizacion = dto.FechaRealizacion,
                    IdDepartamento = departamento,
                };
                repository.Insert(actividades);
                return Ok();
            }
            return BadRequest(resultado.Errors.Select(x => x.ErrorMessage));
        }

        [HttpGet("departamentos")]
        public IActionResult GetByDepartamentos()
        {
            var departamento = ObtenerIdDepartamento();
            var actividades = repository.GetAll().Where(x => x.IdDepartamento >= departamento);
            if (actividades == null || !actividades.Any())
            {
                return NotFound();
            }

            var actividadesDTO = actividades.Select(x => new ActividadDTO
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                Titulo = x.Titulo,
                Estado = x.Estado,
                FechaActualizacion = x.FechaActualizacion,
                FechaCreacion = x.FechaCreacion,
                FechaRealizacion = x.FechaRealizacion,
                IdDepartamento = x.IdDepartamento
            });

            return Ok(actividadesDTO);
        }

        [HttpGet("departamento")]
        public IActionResult GetByDepartamento()
        {
            var departamento = ObtenerIdDepartamento();
            var actividades = repository.GetAll().Where(x => x.IdDepartamento == departamento);
            if (actividades == null || !actividades.Any())
            {
                return NotFound();
            }

            var actividadesDTO = actividades.Select(x => new ActividadDTO
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                Titulo = x.Titulo,
                Estado = x.Estado,
                FechaActualizacion = x.FechaActualizacion,
                FechaCreacion = x.FechaCreacion,
                FechaRealizacion = x.FechaRealizacion,
                IdDepartamento = x.IdDepartamento
            });

            return Ok(actividadesDTO);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var departamento = ObtenerIdDepartamento();
            var actividad = repository.Get(id);
            if (actividad == null)
            {
                return NotFound();
            }

            if (actividad.IdDepartamento == departamento)
            {
                var actividadDTO = new ActividadDTO
                {
                    Id = actividad.Id,
                    Descripcion = actividad.Descripcion,
                    Titulo = actividad.Titulo,
                    Estado = actividad.Estado,
                    FechaActualizacion = actividad.FechaActualizacion,
                    FechaCreacion = actividad.FechaCreacion,
                    FechaRealizacion = actividad.FechaRealizacion,
                    IdDepartamento = actividad.IdDepartamento
                };
                return Ok(actividadDTO);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(ActividadDTO dto)
        {
            ActividadValidator validator = new ActividadValidator();
            var departamento = ObtenerIdDepartamento();
            var resultado = validator.Validate(dto);
            if (resultado.IsValid)
            {
                var entidadactividad = repository.Get(dto.Id);
                if (entidadactividad == null || entidadactividad.Estado == 2)
                {
                    return NotFound();
                }
                else
                {
                    if (departamento == dto.IdDepartamento)
                    {
                        entidadactividad.Titulo = dto.Titulo;
                        entidadactividad.Descripcion = dto.Descripcion;
                        entidadactividad.FechaCreacion = dto.FechaCreacion;
                        entidadactividad.FechaRealizacion = dto.FechaRealizacion;
                        entidadactividad.FechaActualizacion = DateTime.Now;
                        repository.Update(entidadactividad);
                        return Ok();
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
            }
            return BadRequest(resultado.Errors.Select(x => x.ErrorMessage));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var departamento = ObtenerIdDepartamento();
            var entidadactividad = repository.Get(id);
            if (entidadactividad == null || entidadactividad.Estado == 2)
            {
                return NotFound();
            }
            if (entidadactividad.IdDepartamento == departamento)
            {
                entidadactividad.Estado = 2;
                entidadactividad.FechaActualizacion = DateTime.Now;
                repository.Update(entidadactividad);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}