using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Entities
{
    /// <summary>
    /// Represent <c>User</c> class.
    /// </summary>
    public class User : Entity
    {
        /// <summary>
        /// Represent user email.
        /// </summary>
        public EmailObject Email { get; protected set; } = default!;

        /// <summary>
        /// Represent user password.
        /// </summary>
        public string Password { get; protected set; } = default!;

        protected User()
        {
        }

        /// <summary>
        /// Build <c>User</c> object.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="password">User password.</param>
        public User(string email, string password)
        {
            Email = new EmailObject(email);
            Password = password;

            Validate();
        }

        /// <summary>
        /// Validate the user class.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a user class is valid.
        ///     <c>false</c> if a user class is invalid.
        /// </returns>
        public bool Validate() => Validate(this, new UserValidator<User>());

        /// <summary>
        /// Add Encrypted Password for User.
        /// </summary>
        /// <param name="passwordEncrypted">Encrypted password.</param>
        public void Encript(string passwordEncrypted) => Password = passwordEncrypted;
    }
}