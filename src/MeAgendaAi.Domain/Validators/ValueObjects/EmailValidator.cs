using FluentValidation;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Validators.ValueObjects
{

    /// <summary>
    /// Validation of <see cref="EmailObject"/>.
    /// </summary>
    public class EmailValidator : AbstractValidator<EmailObject>
    {
        public EmailValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-mail cannot be empty")
                .EmailAddress().WithMessage("Invalid e-mail");
        }
    }
}
