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
    [Authorize(Roles = "Administrador")]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentosController : ControllerBase
    {
        public readonly DepartamentosRepository repository;
        private readonly ActividadesRepository actividadesRepository;

        public DepartamentosController(DepartamentosRepository departamentosRepository, ActividadesRepository actividadesRepository)
        {
            repository = departamentosRepository;
            this.actividadesRepository = actividadesRepository;
        }

        [HttpPost]
        public IActionResult Post(DepartamentoDTO dto)
        {
            DepartamentoValidator validador = new();
            var resultado = validador.Validate(dto);
            if (resultado.IsValid)
            {
                Departamentos departamentos = new Departamentos()
                {
                    Id = 0,
                    Nombre = dto.Nombre,
                    Username = dto.Username,
                    Password = dto.Password,
                    IdSuperior = dto.IdSuperior
                };
                repository.Insert(departamentos);
                return Ok(departamentos);
            }
            return BadRequest(resultado.Errors.Select(x => x.ErrorMessage));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var departamento = repository.GetSubdepartamentos(id).Select(x => new DepartamentoDTO()
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
        public IActionResult Put(DepartamentoDTO dto)
        {
            DepartamentoValidator validator = new DepartamentoValidator();
            var resultado = validator.Validate(dto);
            if (resultado.IsValid)
            {
                var entidaddepartamento = repository.Get(dto.Id);
                if (entidaddepartamento == null)
                {
                    return NotFound();
                }
                else
                {
                    var encrypt = Encriptacion.StringToSHA512(dto.Password);
                    entidaddepartamento.Nombre = dto.Nombre;
                    entidaddepartamento.Username = dto.Username;
                    entidaddepartamento.Password = encrypt;
                    entidaddepartamento.IdSuperior = dto.IdSuperior;
                    repository.Update(entidaddepartamento);
                    return Ok();
                }
            }
            return BadRequest(resultado.Errors.Select(x => x.ErrorMessage));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entidaddepartamento = repository.Get(id);
            if (entidaddepartamento == null)
            {
                return NotFound();
            }
            var actividadesdepartamento = actividadesRepository.GetAll().Where(x=>x.IdDepartamento == id);
            foreach (var actividad in actividadesdepartamento)
            {
                actividadesRepository.Delete(actividad);
            }
            repository.Delete(entidaddepartamento);
            return Ok();
        }
    }
}
