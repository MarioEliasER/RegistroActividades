using FluentValidation;
using RegistroActividades.Models.DTOs;

namespace RegistroActividades.Models.Validators
{
    public class ActividadValidator : AbstractValidator<ActividadDTO>
    {
        public ActividadValidator()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage("El titulo no puede estar vacío.");
            RuleFor(x => x.Estado).GreaterThanOrEqualTo(0).LessThanOrEqualTo(2).WithMessage("El estado no puede ser menor que 0 ni mayor que 2.");
            RuleFor(x => x.IdDepartamento).NotNull().WithMessage("El departamento no puede estar vacío.");
        }
    }
}
