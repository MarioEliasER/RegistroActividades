using FluentValidation;
using RegistroActividades.Models.DTOs;
using RegistroActividades.Models.Entities;

namespace RegistroActividades.Models.Validators
{
    public class DepartamentoValidator : AbstractValidator<DepartamentoDTO>
    {
        public DepartamentoValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre del departamento no puede estar vacío.");
            RuleFor(x => x.Username).NotEmpty().WithMessage("El nombre de usuario del departamento no puede estar vacío.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("La contraseña del departamento no puede estar vacía.");
            
        }
    }
}
