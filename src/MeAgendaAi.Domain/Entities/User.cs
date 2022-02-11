using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Entities
{
    public class User : Entity
    {
        public EmailObject Email { get; protected set; } = default!;
        public string Password { get; protected set; } = default!;
        protected User()
        {
        }
        public User(string email, string password)
        {
            Email = new EmailObject(email);
            Password = password;

            Validate(this, new UserValidator<User>());
        }

        public void Encript(string password) => Password = password;
    }
}
