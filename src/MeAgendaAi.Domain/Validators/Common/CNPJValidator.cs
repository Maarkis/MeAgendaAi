using FluentValidation;

namespace MeAgendaAi.Domains.Validators.Common
{
    public class CNPJValidator : AbstractValidator<string>
    {
        public CNPJValidator()
        {
            RuleFor(prop => prop)
                .NotNull()
                .WithMessage("CPNJ cannot be null")
                .NotEmpty()
                .WithMessage("CPNJ cannot be empty");
        }
    }
}
