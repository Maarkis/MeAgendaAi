using FluentValidation;
using System.Text.RegularExpressions;

namespace MeAgendaAi.Domains.Validators.Common
{
    internal class CPFValidator : AbstractValidator<string>
    {
        public CPFValidator()
        {
            RuleFor(prop => prop)
                .NotNull()
                .WithMessage("CPF cannot be null")
                .NotEmpty()
                .WithMessage("CPF cannot be empty");
        }
    }
}
