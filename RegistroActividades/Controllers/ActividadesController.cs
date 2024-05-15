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

        ActividadValidator validator = new ActividadValidator();

        [HttpPost]
        public IActionResult Post(ActividadDTO dto)
        {

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
                    IdDepartamento = dto.IdDepartamento,
                };
                repository.Insert(actividades);
                return Ok(actividades);
            }
            return BadRequest(resultado.Errors.Select(x => x.ErrorMessage));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var actividad = repository.Get(id);
            if (actividad == null)
            {
                return NotFound();
            }

            var actividadDTO = new ActividadDTO
            {
                Id = actividad.Id,
                Descripcion = actividad.Descripcion,
                Titulo=actividad.Titulo,
                Estado=actividad.Estado,
                FechaActualizacion = actividad.FechaActualizacion,
                FechaCreacion = actividad.FechaCreacion,
                FechaRealizacion = actividad.FechaRealizacion,
                IdDepartamento = actividad.IdDepartamento
            };
            return Ok(actividadDTO);
        }

        [HttpPut("{id}")]
        public IActionResult Put(ActividadDTO dto)
        {
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
                    entidadactividad.Titulo = dto.Titulo;
                    entidadactividad.Descripcion = dto.Descripcion;
                    entidadactividad.IdDepartamento = dto.IdDepartamento;
                    entidadactividad.FechaCreacion = dto.FechaCreacion;
                    entidadactividad.FechaRealizacion = dto.FechaRealizacion;
                    entidadactividad.FechaActualizacion = DateTime.Now;
                    repository.Update(entidadactividad);
                    return Ok();
                }
            }
            return BadRequest(resultado.Errors.Select(x => x.ErrorMessage));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entidadactividad = repository.Get(id);
            if (entidadactividad == null || entidadactividad.Estado == 2)
            {
                return NotFound();
            }
            entidadactividad.Estado = 2;
            entidadactividad.FechaActualizacion = DateTime.Now;
            repository.Update(entidadactividad);
            return Ok();
        }
    }
}
