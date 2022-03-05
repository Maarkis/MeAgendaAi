using MeAgendaAi.Domains.Validators.ValueObjects;

namespace MeAgendaAi.Domains.ValueObjects
{
    /// <summary>
    /// Represents E-mail.
    /// </summary>
    public class EmailObject : ValueObjects
    {
        public string Email { get; protected set; } = default!;

        /// <summary>
        /// Constructs a class that represents an email.
        /// </summary>
        /// <param name="email">E-mail.</param>
        public EmailObject(string email) : base()
        {
            Email = email;
            Validate(this, new EmailValidator());
        }
    }
}
