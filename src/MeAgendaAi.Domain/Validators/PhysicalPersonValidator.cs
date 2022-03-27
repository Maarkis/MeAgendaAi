using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Validators.Common;

namespace MeAgendaAi.Domains.Validators
{
    public class PhysicalPersonValidator : UserValidator<PhysicalPerson>
    {
        public PhysicalPersonValidator() : base(includeSurname: true)
        {
            RuleFor(prop => prop.CPF).SetValidator(new CPFValidator());
            RuleFor(prop => prop.RG).SetValidator(new RGValidator());                    
        }
    }
}
