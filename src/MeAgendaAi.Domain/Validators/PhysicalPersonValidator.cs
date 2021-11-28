using FluentValidation;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Validators.Common;
using MeAgendaAi.Domains.Validators.ValueObjects;

namespace MeAgendaAi.Domains.Validators
{
    public class PhysicalPersonValidator : UserValidator<PhysicalPerson>
    {
        public PhysicalPersonValidator()
        {
            RuleFor(prop => prop.Name).SetValidator(new NameValidator());
            RuleFor(prop => prop.CPF).SetValidator(new CPFValidator());
            RuleFor(prop => prop.RG).SetValidator(new RGValidator());
            RuleFor(prop => prop.Email).SetValidator(new EmailValidator());            
        }
    }
}
