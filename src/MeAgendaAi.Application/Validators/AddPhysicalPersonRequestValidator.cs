﻿using FluentValidation;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Application.Validators
{
    public class AddPhysicalPersonRequestValidator : AbstractValidator<AddPhysicalPersonRequest>
    {
        const string ErrorMessageEmpty = "Can't be empty";
        const string ErrorMessageNull = "Can't be null";
        public AddPhysicalPersonRequestValidator()
        {
            RuleFor(prop => prop.Name).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull().WithMessage(ErrorMessageNull);
            RuleFor(prop => prop.Surname).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull().WithMessage(ErrorMessageNull);
            RuleFor(prop => prop.Email).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull().WithMessage(ErrorMessageNull);
            RuleFor(prop => prop.Password).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull().WithMessage(ErrorMessageNull);
            RuleFor(prop => prop.ConfirmPassword).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull().WithMessage(ErrorMessageNull);
            RuleFor(prop => prop.RG).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull().WithMessage(ErrorMessageNull);
            RuleFor(prop => prop.CPF).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull().WithMessage(ErrorMessageNull);            
        }
    }
}

