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

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult Post(DepartamentoDTOAdd dto)
        {
            var iddepa = ObtenerIdDepartamento();
            var usuario = departamentosRepository.Get(iddepa);
            if (usuario.IdSuperior != null) return Forbid();
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
            var departamentos = departamentosRepository.GetAll().Where(x => x.Username.EndsWith("@apiequipo10.com")).Select(x => new DepartamentoDTO()
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
        public async Task<IActionResult> Delete(int id)
        {
            var context = HttpContext;
            var idusuario = ObtenerIdDepartamento();
            var usuario = await departamentosRepository.GetIncludeActividades(id);
            if (usuario == null)
            {
                return BadRequest();
            }
            if (usuario.IdSuperior == null)
            {
                return Forbid();
            }
            var actividades = usuario.Actividades.ToList();

            foreach (var actividad in actividades)
            {
                actividad.Estado = 2;
                actividadesRepository.Update(actividad);
            }

            var scopeFactory = HttpContext.RequestServices.GetRequiredService<IServiceScopeFactory>();

            _ = Task.Run(async () =>
            {
                // Crear un nuevo scope para la tarea en segundo plano
                using (var scope = scopeFactory.CreateScope())
                {
                    var scopedDepartamentosRepository = scope.ServiceProvider.GetRequiredService<DepartamentosRepository>();

                    try
                    {
                        await Task.Delay(7000); // Espera de 11 segundos

                        scopedDepartamentosRepository.DeleteDepartment(id);
                    }
                    catch (Exception ex)
                    {
                        // Maneja la excepción, registra o envía notificación
                        Console.WriteLine($"Error during background operation: {ex.Message}");
                    }
                }
            });
            return Ok();
        }
    }
}
