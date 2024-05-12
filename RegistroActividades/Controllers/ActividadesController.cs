using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegistroActividades.Models.DTOs;
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

        [HttpPost]
        public IActionResult Post(ActividadDTO dto)
        {
            ActividadValidator validator = new ActividadValidator();
            var resultado = validator.Validate(dto);
            if (resultado.IsValid)
            {
                //Mappear
                return Ok();
            }
            return BadRequest(resultado.Errors.Select(x => x.ErrorMessage));
        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            return Ok();
        }

        [HttpPut]
        public IActionResult Put(ActividadDTO dto)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
