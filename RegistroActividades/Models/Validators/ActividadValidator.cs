using FluentValidation;
using RegistroActividades.Models.DTOs;

namespace RegistroActividades.Models.Validators
{
    public class ActividadValidator : AbstractValidator<ActividadDTO>
    {
        public ActividadValidator()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage("El titulo no puede estar vacío.");
            RuleFor(x => x.Estado).GreaterThanOrEqualTo(3).LessThanOrEqualTo(3).WithMessage("El estado no puede ser menor que 0 ni mayor que 3.");
            RuleFor(x => x.IdDepartamento).NotNull().WithMessage("El departamento no puede estar vacío.");
        }
    }
}
