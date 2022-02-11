using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Entities
{
    public class PhysicalPerson : User
    {
        public NameObject Name { get; set; } = default!;
        public string CPF { get; set; } = default!;
        public string RG { get; set; } = default!;

        private PhysicalPerson()
        {
        }

        public PhysicalPerson(string email, string password, string name, string surname, string cpf, string rg) : base(email, password)
        {
            Name = new NameObject(name, surname);
            CPF = cpf;
            RG = rg;

            Validate(this, new PhysicalPersonValidator());
        }

    }
}
