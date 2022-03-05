using FluentValidation;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Validators
{
    /// <summary>
    /// Validation of <see cref="NameObject"/>.
    /// </summary>
    public class NameValidator : AbstractValidator<NameObject>
    {
        public NameValidator(bool includeSurname = true)
        {
            RuleFor(prop => prop.Name)
                .NotEmpty().WithMessage("Name cannot be empty")
                .MinimumLength(3).WithMessage("Name must contain at least 3 characters")
                .MaximumLength(60).WithMessage("Name must contain a maximum  of 60 characters");

            if (includeSurname)
            {
                RuleFor(prop => prop.Surname)                   
                  .NotEmpty().WithMessage("Surname cannot be empty")
                  .MinimumLength(3).WithMessage("Surname must contain at least 3 characters")
                  .MaximumLength(80).WithMessage("Surname must contain a maximum  of 80 characters");
            }
        }
    }
}
