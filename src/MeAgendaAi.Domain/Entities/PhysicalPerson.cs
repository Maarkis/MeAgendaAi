using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Entities
{
    /// <summary>
    /// Represent <c>Physical Person</c> class.
    /// </summary>
    public class PhysicalPerson : User
    {
        /// <summary>
        /// Represent Physical Person name.
        /// </summary>
        public NameObject Name { get; protected set; } = default!;

        /// <summary>
        /// Represents of Registration of physical person.
        /// </summary>
        public string CPF { get; protected set; } = default!;

        /// <summary>
        /// Represents of Id Card of Physical Person.
        /// </summary>
        public string RG { get; protected set; } = default!;

        private PhysicalPerson()
        {
        }

        /// <summary>
        /// Build <c>PhysicalPerson</c> object.
        /// </summary>
        /// <param name="email">Email of physical person.</param>
        /// <param name="password">Password of physical person.</param>
        /// <param name="name">Name of physical person.</param>
        /// <param name="surname">Surname of physical person.</param>
        /// <param name="cpf">Registration of physical person.</param>
        /// <param name="rg">Id Card of Physical Person.</param>
        public PhysicalPerson(string email, string password, string name, string surname, string cpf, string rg) : base(email, password)
        {
            Name = new NameObject(name, surname);
            CPF = cpf;
            RG = rg;

            Validate();
        }

        /// <summary>
        /// Validate the PhysicalPerson class.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a PhysicalPerson class is valid.
        ///     <c>false</c> if a PhysicalPerson class is invalid.
        /// </returns>
        public new bool Validate() => Validate(this, new PhysicalPersonValidator());
    }
}