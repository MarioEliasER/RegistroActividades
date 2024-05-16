using FluentValidation;
using RegistroActividades.Models.DTOs;

namespace RegistroActividades.Models.Validators
{
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("El usuario no puede estar vacío");
            RuleFor(x => x.Password).NotEmpty().WithMessage("La contraseña no puede estar vacía.");
        }
    }
}
