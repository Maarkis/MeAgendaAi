using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Validate(this, new CompanyValidator());
        }
    }
}
