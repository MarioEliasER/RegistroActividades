﻿using FluentValidation;
using RegistroActividades.Models.DTOs;

namespace RegistroActividades.Models.Validators
{
    public class DepartamentoAddValidator : AbstractValidator<DepartamentoDTOAdd>
    {
        public DepartamentoAddValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre del departamento no puede estar vacío.");
            RuleFor(x => x.Username).NotEmpty().WithMessage("El nombre de usuario del departamento no puede estar vacío.");
            //RuleFor(x => x.Password).NotEmpty().WithMessage("La contraseña del departamento no puede estar vacía.");
            RuleFor(x => x.Username).Must(ContieneDominioValido).WithMessage("El nombre de usuario del departamento debe terminar con apiequipo10.com");
        }

        private bool ContieneDominioValido(string email)
        {
            return email != null && email.EndsWith("apiequipo10.com");
        }
    }
}
