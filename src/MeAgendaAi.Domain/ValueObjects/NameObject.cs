using MeAgendaAi.Domains.Validators;

namespace MeAgendaAi.Domains.ValueObjects
{
    public class NameObject : ValueObjects
    {
        public string Name { get; protected set; } = default!;
        public string Surname { get; protected set; } = default!;
        public string FullName => $"{Name} {Surname}".Trim();

        protected NameObject()
        {
        }

        public NameObject(string name, string surname) : base()
        {
            Name = name;
            Surname = surname;
            Validate(this, new NameValidator(includeSurname: true));
        }

        public NameObject(string name) : base()
        {
            Name = name;
            Validate(this, new NameValidator(includeSurname: false));
        }
    }
}