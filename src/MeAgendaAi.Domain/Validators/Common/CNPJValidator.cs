using FluentValidation;

namespace MeAgendaAi.Domains.Validators.Common
{
    public class CNPJValidator : AbstractValidator<string>
    {
        public CNPJValidator()
        {
            RuleFor(prop => prop)
                .NotNull()
                .NotEmpty()
                .WithMessage("CPNJ cannot be empty");
        }
    }
}
