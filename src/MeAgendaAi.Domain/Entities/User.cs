using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Entities
{
    public class User : Entity
    {
        public Name Name { get; protected set; } = default!;
        public Email Email { get; protected set; } = default!;
        public string Password { get; protected set; } = default!;
        public bool IsActive { get; protected set; }

        protected User()
        {
        }

        public User(string email, string password, string name)
        {
            Email = new Email(email);
            Password = password;
            Name = new Name(name);
            IsActive = false;

            Validate(includeSurname: false);
        }

        public User(string email, string password, string name, string surname)
        {
            Email = new Email(email);
            Password = password;
            Name = new Name(name, surname);
            IsActive = false;

            Validate(includeSurname: true);
        }

        public bool Validate(bool includeSurname) => Validate(this, new UserValidator<User>(includeSurname));

        public void Encrypt(string password) => Password = password;

        public void Active()
        {
            IsActive = true;
            UpdatedAt();
        }
    }
}