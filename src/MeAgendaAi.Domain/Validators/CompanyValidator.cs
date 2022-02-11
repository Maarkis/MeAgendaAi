using FluentValidation;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Validators.Common;

namespace MeAgendaAi.Domains.Validators
{
    public class CompanyValidator : UserValidator<Company>
    {
        public CompanyValidator()
        {
            RuleFor(prop => prop.Name).SetValidator(new NameValidator(includeSurname: false));
            RuleFor(prop => prop.CNPJ).SetValidator(new CNPJValidator());
            RuleFor(prop => prop.LimitCancelHours)
                .NotEmpty().NotNull().WithMessage("Limit cancel hours cannot be empty");
            RuleFor(prop => prop.Description)
                .NotEmpty().NotNull().WithMessage("Description cannot be empty")
                .MinimumLength(3).WithMessage("Description must contain at least 3 characters")
                .MaximumLength(160).WithMessage("Description must contain a maximum of 160 characters");
        }
    }
}
