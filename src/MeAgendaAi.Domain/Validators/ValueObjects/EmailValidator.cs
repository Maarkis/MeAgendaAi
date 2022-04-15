using FluentValidation;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Validators.ValueObjects
{
    public class EmailValidator : AbstractValidator<Email>
    {
        public EmailValidator()
        {
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("E-mail cannot be empty")
                .EmailAddress().WithMessage("Invalid e-mail");
        }
    }
}
