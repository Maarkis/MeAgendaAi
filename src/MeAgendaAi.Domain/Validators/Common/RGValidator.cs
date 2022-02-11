using FluentValidation;
using System.Text.RegularExpressions;

namespace MeAgendaAi.Domains.Validators.Common
{
    public class RGValidator : AbstractValidator<string>
    {
        public RGValidator()
        {
            RuleFor(prop => prop)
                .NotNull()
                .NotEmpty()
                .WithMessage("RG cannot be empty");
        }
    }
}
