using FluentValidation;
using MeAgendaAi.Domains.Entities;

namespace MeAgendaAi.Domains.Validators
{
    public class PhysicalPersonValidator : UserValidator<PhysicalPerson>
    {
        public PhysicalPersonValidator() : base(includeSurname: true)
        {
            RuleFor(prop => prop.Cpf)
                .NotNull()
                .WithMessage("CPF cannot be null")
                .NotEmpty()
                .WithMessage("CPF cannot be empty");
            RuleFor(prop => prop.Rg)
                .NotNull()
                .WithMessage("RG cannot be null")
                .NotEmpty()
                .WithMessage("RG cannot be empty");
        }
    }
}