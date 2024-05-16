using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegistroActividades.Helpers;
using RegistroActividades.Models.DTOs;
using RegistroActividades.Models.Validators;
using RegistroActividades.Repositories;
using System.Security.Claims;

namespace RegistroActividades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DepartamentosRepository repository;
        private readonly JwtHelper jwtHelper;

        public LoginController(DepartamentosRepository repo, JwtHelper helper)
        {
            repository = repo;
            jwtHelper = helper;
        }

        [HttpPost]
        public IActionResult Login(LoginDTO dto)
        {
            LoginValidator validator = new LoginValidator();
            var resultado = validator.Validate(dto);
            if (resultado.IsValid)
            {
                var encrypt = Encriptacion.StringToSHA512(dto.Password);
                var us = repository.GetAll().FirstOrDefault(x => x.Username == dto.Username && x.Password == encrypt);
                if (us == null)
                {
                    return Unauthorized();
                }

                var token = jwtHelper.GetToken(us.Username, us.IdSuperior == null ? "Administrador" : "Departamento", us.Id, new List<Claim>
                { new Claim("Id", us.Id.ToString())});
                return Ok(token);
            }
            return BadRequest(resultado.Errors.Select(x => x.ErrorMessage));
        }
    }
}