using FluentValidation;

namespace MeAgendaAi.Domains.Validators.Common
{
    /// <summary>
    /// Validation of ID Card (RG).
    /// </summary>
    public class RGValidator : AbstractValidator<string>
    {
        public RGValidator()
        {
            RuleFor(prop => prop)
                .NotNull()
                .WithMessage("RG cannot be null")
                .NotEmpty()
                .WithMessage("RG cannot be empty");
        }
    }
}
