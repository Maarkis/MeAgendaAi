using FluentValidation;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Validators.ValueObjects;

namespace MeAgendaAi.Domains.Validators
{
    public class UserValidator<T> : AbstractValidator<T> where T : User
    {
        const int LENGTH_PASSWORD_MINIMUM = 06;
        const int LENGTH_PASSWORD_MAXIMUM = 32;
        public UserValidator()
        {
            RuleFor(prop => prop.Email)
                .SetValidator(new EmailValidator());
            RuleFor(prop => prop.Password)
                .NotEmpty().WithMessage("Password cannot be empty")
                .MinimumLength(LENGTH_PASSWORD_MINIMUM).WithMessage($"Password must contain at least {LENGTH_PASSWORD_MINIMUM} characters")
                .MaximumLength(LENGTH_PASSWORD_MAXIMUM).WithMessage($"Password must contain a maximum of {LENGTH_PASSWORD_MAXIMUM} characters");
        }
    }
}
