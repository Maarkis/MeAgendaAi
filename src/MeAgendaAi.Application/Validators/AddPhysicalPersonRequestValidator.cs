using FluentValidation;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Application.Validators
{
    public class AddPhysicalPersonRequestValidator : AbstractValidator<AddPhysicalPersonRequest>
    {
        const string ERROR_MESSAGE_EMPTY = "Can't be empty";
        const string ERROR_MESSAGE_NULL = "Can't be null";
        public AddPhysicalPersonRequestValidator()
        {
            RuleFor(prop => prop.Name).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.Surname).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.Email).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.Password).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.ConfirmPassword).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.RG).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.CPF).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);            
        }
    }
}

