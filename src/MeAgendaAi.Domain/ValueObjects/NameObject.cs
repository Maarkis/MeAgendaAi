using MeAgendaAi.Domains.Validators;

namespace MeAgendaAi.Domains.ValueObjects
{
    /// <summary>
    /// Represents name.
    /// </summary>
    public class NameObject : ValueObjects
    {
        public string Name { get; protected set; } = default!;
        public string Surname { get; protected set; } = default!;
        public string FullName => $"{Name} {Surname}".Trim();

        protected NameObject()
        {
        }

        /// <summary>
        /// Constructs a class that represents a name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="surname">Surname.</param>
        public NameObject(string name, string surname) : base()
        {
            Name = name;
            Surname = surname;
            Validate(this, new NameValidator(includeSurname: true));
        }

        /// <summary>
        /// Constructs a class that represents a name.
        /// </summary>
        /// <param name="name">Name.</param>
        public NameObject(string name) : base()
        {
            Name = name;
            Validate(this, new NameValidator(includeSurname: false));
        }
    }
}