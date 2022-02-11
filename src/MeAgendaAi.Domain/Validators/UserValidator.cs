using FluentValidation;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Validators.ValueObjects;

namespace MeAgendaAi.Domains.Validators
{
    public class UserValidator<T> : AbstractValidator<T> where T : User
    {
        public UserValidator()
        {
            RuleFor(prop => prop.Email)
                .SetValidator(new EmailValidator());
            RuleFor(prop => prop.Password)
                .NotEmpty().WithMessage("Password cannot be empty");
        }
    } 
}
