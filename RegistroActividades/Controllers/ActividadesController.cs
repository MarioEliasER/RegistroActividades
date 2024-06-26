﻿using Microsoft.AspNetCore.Http;
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
                //throw new UnauthorizedAccessException("El usuario no está autenticado.");
                return 0;
            }
        }


        [HttpGet("departamentos")]
        public IActionResult GetByDepartamentos()
        {
            var departamento = ObtenerIdDepartamento();
            if (departamento == 0)
            {
                var ActividadesDTO = new List<ActividadDTO>();
                return Ok(ActividadesDTO);
            }
            var actividades = repository.GetAll().Where(x => x.IdDepartamento >= departamento);
            if (actividades == null || !actividades.Any())
            {
                var ActividadesDTO = new List<ActividadDTO>();
                return Ok(ActividadesDTO);
            }

            var actividadesDTO = actividades.Select(x => new ActividadDTO
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                Titulo = x.Titulo,
                Estado = x.Estado,
                FechaActualizacion = x.FechaActualizacion,
                FechaCreacion = x.FechaCreacion,
                FechaRealizacion = x.FechaRealizacion.HasValue ? x.FechaRealizacion.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                IdDepartamento = x.IdDepartamento,
                Imagen = ConvertImageToBase64($"wwwroot/imagenes/{x.Id}.jpg")
            });
            return Ok(actividadesDTO);
        }

        [HttpGet("departamento")]
        public async Task<IActionResult> GetAllActividades(DateTime? fecha)
        {
            var departamento = ObtenerIdDepartamento();
            var actividades = await repository.GetAllActividadesPublicadasAsync(departamento, fecha ?? new DateTime(2000, 01, 01));
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
                FechaRealizacion = x.FechaRealizacion.HasValue ? x.FechaRealizacion.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                IdDepartamento = x.IdDepartamento
            });
            return Ok(actividadesDTO);
        }

        [HttpGet("departamento/borradores")]
        public IActionResult GetBorradores()
        {
            var departamento = ObtenerIdDepartamento();
            var actividades = repository.GetAll().Where(x => x.IdDepartamento == departamento && x.Estado == 0);
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
                FechaRealizacion = x.FechaRealizacion.HasValue ? x.FechaRealizacion.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                IdDepartamento = x.IdDepartamento,
                Imagen = ConvertImageToBase64($"wwwroot/imagenes/{x.Id}.jpg")
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
                    FechaRealizacion = actividad.FechaRealizacion.HasValue ? actividad.FechaRealizacion.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    IdDepartamento = actividad.IdDepartamento,
                    Imagen = ConvertImageToBase64($"wwwroot/imagenes/{actividad.Id}.jpg")
                };
                return Ok(actividadDTO);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public IActionResult Post(ActividadDTO dto)
        {
            if (!System.IO.Directory.Exists("wwwroot/imagenes"))
            {
                System.IO.Directory.CreateDirectory("wwwroot/imagenes");
            }
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
                    FechaRealizacion = dto.FechaRealizacion.HasValue ? DateOnly.FromDateTime(dto.FechaRealizacion.Value) : (DateOnly?)null,
                    IdDepartamento = departamento,
                };
                repository.Insert(actividades);
                string imagePath = $"wwwroot/imagenes/{actividades.Id}.jpg";
                byte[] imageBytes = Convert.FromBase64String(dto.Imagen);
                System.IO.File.WriteAllBytes(imagePath, imageBytes);
                return Ok();
            }
            return BadRequest(resultado.Errors.Select(x => x.ErrorMessage));
        }

        [HttpPut("{id}")]
        public IActionResult Put(ActividadDTO dto)
        {
            if (!System.IO.Directory.Exists("wwwroot/imagenes"))
            {
                System.IO.Directory.CreateDirectory("wwwroot/imagenes");
            }
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
                        entidadactividad.Estado = dto.Estado;
                        entidadactividad.FechaRealizacion = dto.FechaRealizacion.HasValue ? DateOnly.FromDateTime(dto.FechaRealizacion.Value) : (DateOnly?)null;
                        entidadactividad.FechaActualizacion = DateTime.UtcNow;
                        repository.Update(entidadactividad);
                        if (!string.IsNullOrWhiteSpace(dto.Imagen))
                        {
                            string imageName = $"{dto.Id}_apiequipo10.jpg";
                            string imagePath = $"wwwroot/imagenes/{dto.Id}.jpg";
                            byte[] imageBytes = Convert.FromBase64String(dto.Imagen);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                        }

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
            if (entidadactividad.IdDepartamento == departamento || departamento == 1)
            {
                entidadactividad.Estado = 2;
                entidadactividad.FechaActualizacion = DateTime.UtcNow;
                repository.Update(entidadactividad);

                string imagePath = $"wwwroot/imagenes/{entidadactividad.Id}.jpg";
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("ConvertImage")]
        public string ConvertImageToBase64(string imagePath)
        {
            if (System.IO.File.Exists(imagePath))
            {
                byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                return base64ImageRepresentation;
            }
            return "";
        }
    }
}
