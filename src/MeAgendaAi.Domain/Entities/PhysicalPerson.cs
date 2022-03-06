using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Entities
{
    public class PhysicalPerson : User
    {
        public string CPF { get; protected set; } = default!;
        public string RG { get; protected set; } = default!;

        private PhysicalPerson()
        {
        }

        public PhysicalPerson(string email, string password, string name, string surname, string cpf, string rg) : base(email, password, name)
        {
            Name = new NameObject(name, surname);
            CPF = cpf;
            RG = rg;

            Validate();
        }

        public bool Validate() => Validate(this, new PhysicalPersonValidator());
    }
}