using FluentValidation;
using RegistroActividades.Models.DTOs;

namespace RegistroActividades.Models.Validators
{
    public class ActividadValidator : AbstractValidator<ActividadDTO>
    {
        public ActividadValidator()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage("El titulo no puede estar vacío.");
        }
    }
}
