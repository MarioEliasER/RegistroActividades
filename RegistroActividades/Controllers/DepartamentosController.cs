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
    public class DepartamentosController : ControllerBase
    {
        public readonly DepartamentosRepository repository;

        public DepartamentosController(DepartamentosRepository repo)
        {
            repository = repo;
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
            var departamento = repository.Get(id);
            if (departamento == null)
            {
                return NotFound();
            }

            var departamentoDTO = new DepartamentoDTO()
            {
                Id = departamento.Id,
                IdSuperior = departamento.IdSuperior,
                Username = departamento.Username,
                Password = departamento.Password,
                Nombre = departamento.Nombre
            };
            return Ok(departamentoDTO);
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
                    entidaddepartamento.Nombre = dto.Nombre;
                    entidaddepartamento.Username = dto.Username;
                    entidaddepartamento.Password = dto.Password;
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
            repository.Delete(entidaddepartamento);
            return Ok();
        }
    }
}
