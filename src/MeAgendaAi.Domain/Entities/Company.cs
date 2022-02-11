using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Entities
{
    public class Company : User
    {
        public NameObject Name { get; set; } = default!;
        public string CNPJ { get; protected set; } = default!;
        public string Description { get; protected set; } = default!;
        public int LimitCancelHours { get; protected set; } = default!;

        protected Company()
        {
        }

        public Company(string email, string password, string name, string cnpj, string description, int limitCancelHours) : base(email, password)
        {
            Name = new NameObject(name);
            CNPJ = cnpj;
            Description = description;
            LimitCancelHours = limitCancelHours;

            Validate();
        }
        public new bool Validate() => Validate(this, new CompanyValidator());
    }
}
