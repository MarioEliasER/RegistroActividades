using FluentValidation;
using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegistroActividades.Helpers;
using RegistroActividades.Models.DTOs;
using RegistroActividades.Models.Entities;
using RegistroActividades.Models.Validators;
using RegistroActividades.Repositories;

namespace RegistroActividades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentosController : ControllerBase
    {
        public readonly DepartamentosRepository departamentosRepository;
        private readonly ActividadesRepository actividadesRepository;

        public DepartamentosController(DepartamentosRepository departamentosRepository, ActividadesRepository actividadesRepository)
        {
            this.departamentosRepository = departamentosRepository;
            this.actividadesRepository = actividadesRepository;
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult Post(DepartamentoDTOAdd dto)
        {
            DepartamentoAddValidator validador = new();
            var resultado = validador.Validate(dto);
            if (resultado.IsValid)
            {
                var encrypt = Encriptacion.StringToSHA512(dto.Password);
                Departamentos departamentos = new Departamentos()
                {
                    Id = 0,
                    Nombre = dto.Nombre,
                    Username = dto.Username,
                    Password = encrypt,
                    IdSuperior = dto.IdSuperior
                };
                departamentosRepository.Insert(departamentos);
                return Ok(departamentos);
            }
            return BadRequest(resultado.Errors.Select(x => x.ErrorMessage));
        }

        [HttpGet]
        public IActionResult GetAllDepartamentos()
        {
            var departamentos = departamentosRepository.GetAll().Where(x => x.Username.Contains("@apiequipo10")).Select(x => new DepartamentoDTO()
            {
                Nombre = x.Nombre,
                Username = x.Username,
                IdSuperior = x.IdSuperior,
                Id = x.Id,
                Password = x.Password,
                DepartamentoSuperior = x.IdSuperiorNavigation?.Nombre
            }).ToList();
            return Ok(departamentos);
        }

        [HttpGet("departamento/{id}")]
        public IActionResult GetDepartamento(int id)
        {
            var depa = departamentosRepository.Get(id);
            if (depa == null)
            {
                return NotFound();
            }
            var dto = new DepartamentoDTO()
            {
                Id = depa.Id,
                Nombre = depa.Nombre,
                Username = depa.Username,
                Password = depa.Password,
                DepartamentoSuperior = depa.IdSuperiorNavigation?.Nombre,
                IdSuperior = depa.IdSuperior
            };
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var departamento = departamentosRepository.GetSubdepartamentos(id).Select(x => new DepartamentoDTO()
            {
                Nombre = x.Nombre,
                Username = x.Username,
                IdSuperior= x.IdSuperior,
                Id = x.Id,
                Password = x.Password,
                DepartamentoSuperior = x.IdSuperiorNavigation?.Nombre
            });
            if (departamento == null)
            {
                return NotFound();
            }
            return Ok(departamento);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Put(DepartamentoDTO dto)
        {
            DepartamentoValidator validator = new DepartamentoValidator();
            var resultado = validator.Validate(dto);
            if (resultado.IsValid)
            {
                var entidaddepartamento = departamentosRepository.Get(dto.Id);
                if (entidaddepartamento == null)
                {
                    return NotFound();
                }
                else
                {
                    entidaddepartamento.Nombre = dto.Nombre;
                    entidaddepartamento.Username = dto.Username;
                    entidaddepartamento.IdSuperior = dto.IdSuperior;
                    departamentosRepository.Update(entidaddepartamento);
                    return Ok();
                }
            }
            return BadRequest(resultado.Errors.Select(x => x.ErrorMessage));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            //var entidaddepartamento = departamentosRepository.Get(id);
            //if (entidaddepartamento == null)
            //{
            //    return NotFound();
            //}
            //var actividadesdepartamento = actividadesRepository.GetAll().Where(x=>x.IdDepartamento == id);
            //if(actividadesdepartamento != null)
            //{
            //    foreach (var actividad in actividadesdepartamento)
            //    {
            //        actividadesRepository.Delete(actividad);
            //    }
            //}

            //var subdepartamentos = departamentosRepository.GetSubdepartamentos(entidaddepartamento.Id);
            //foreach (var departamento in subdepartamentos)
            //{
            //    departamento.IdSuperior = entidaddepartamento.IdSuperior;
            //    departamentosRepository.Update(departamento);
            //}
            //departamentosRepository.Delete(entidaddepartamento);
            //return Ok();
            using (var transaction = departamentosRepository.Context.Database.BeginTransaction())
            {
                try
                {
                    var entidaddepartamento = departamentosRepository.Get(id);
                    if (entidaddepartamento == null)
                    {
                        return NotFound();
                    }

                    var actividadesdepartamento = actividadesRepository.GetAll().Where(x => x.IdDepartamento == id).ToList();
                    if (actividadesdepartamento.Any())
                    {
                        foreach (var actividad in actividadesdepartamento)
                        {
                            actividadesRepository.Delete(actividad);
                        }
                        actividadesRepository.SaveChanges();
                    }

            //        departamentosRepository.Delete(entidaddepartamento);

            //        transaction.Commit();
            //        return Ok();
            //    }
            //    catch
            //    {
            //        transaction.Rollback();
            //        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the department.");
            //    }
            //}
        }
    }
}
