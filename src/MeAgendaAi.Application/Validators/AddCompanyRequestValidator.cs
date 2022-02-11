using FluentValidation;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Application.Validators
{
    public class AddCompanyRequestValidator : AbstractValidator<AddCompanyRequest>
    {
        const string ERROR_MESSAGE_EMPTY = "Can't be empty";
        const string ERROR_MESSAGE_NULL = "Can't be null";
        public AddCompanyRequestValidator()
        {
            RuleFor(prop => prop.Name).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.Email).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.Password).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.ConfirmPassword).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.CNPJ).NotEmpty().NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.Description).NotEmpty().NotNull().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
            RuleFor(prop => prop.LimitCancelHours).NotEmpty().WithMessage(ERROR_MESSAGE_EMPTY).NotNull().WithMessage(ERROR_MESSAGE_NULL);
        }
    }
}
