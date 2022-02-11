using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Validators.Common;

namespace MeAgendaAi.Domains.Validators
{
    public class PhysicalPersonValidator : UserValidator<PhysicalPerson>
    {
        public PhysicalPersonValidator()
        {
            RuleFor(prop => prop.Name).SetValidator(new NameValidator());
            RuleFor(prop => prop.CPF).SetValidator(new CPFValidator());
            RuleFor(prop => prop.RG).SetValidator(new RGValidator());                    
        }
    }
}
