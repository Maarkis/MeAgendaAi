using MeAgendaAi.Domains.Validators.ValueObjects;

namespace MeAgendaAi.Domains.ValueObjects
{
    public class EmailObject : ValueObjects
    {
        public string Email { get; protected set; }

        public EmailObject(string email) : base()
        {
            Email = email;
            Validate(this, new EmailValidator());
        }
    }
}
