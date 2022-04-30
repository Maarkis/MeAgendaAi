using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Entities
{
    public class PhysicalPerson : User
    {
        public string Cpf { get; protected set; } = default!;
        public string Rg { get; protected set; } = default!;

        private PhysicalPerson()
        {
        }

        public PhysicalPerson(string email, string password, string name, string surname, string cpf, string rg) : base(email, password, name, surname)
        {
            Cpf = cpf;
            Rg = rg;

            Validate();
        }

        public bool Validate() => Validate(this, new PhysicalPersonValidator());
    }
}