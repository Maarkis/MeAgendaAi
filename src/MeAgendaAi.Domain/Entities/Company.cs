using MeAgendaAi.Domains.Validators;

namespace MeAgendaAi.Domains.Entities
{
    public class Company : User
    {
        public string CNPJ { get; protected set; } = default!;
        public string Description { get; protected set; } = default!;
        public int LimitCancelHours { get; protected set; } = default!;

        protected Company()
        {
        }

        public Company(string email, string password, string name, string cnpj, string description, int limitCancelHours) : base(email, password, name)
        {
            CNPJ = cnpj;
            Description = description;
            LimitCancelHours = limitCancelHours;

            Validate();
        }

        public bool Validate() => Validate(this, new CompanyValidator());
    }
}