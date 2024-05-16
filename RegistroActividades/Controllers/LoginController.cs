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
            var encrypt = Encriptacion.StringToSHA512(dto.Password);
            var us = repository.GetAll().FirstOrDefault(x => x.Nombre == dto.Username && x.Password == encrypt);
            if (us == null)
            {
                return Unauthorized();
            }

            if (us.Nombre == "Director General")
            {

            }

            var token = jwtHelper.GetToken(us.Nombre, us.Id == 1 ? "Admin" : "Usuario", new List<Claim>
            { new Claim("Id", us.Id.ToString())});
            return Ok(token);
        }
    }
}
