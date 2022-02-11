using MeAgendaAi.Domains.Validators;

namespace MeAgendaAi.Domains.ValueObjects
{
    public class NameObject : ValueObjects
    {
        public string Name { get; private set; }
        public string Surname { get; private set; } = default!;
        public string FullName => $"{Name} {Surname}".Trim();

        public NameObject(string name)
        {
            Name = name;
            Validate(this, new NameValidator(includeSurname: false));
        }

        public NameObject(string name, string surname)
        {
            Name = name;
            Surname = surname;
            Validate(this, new NameValidator(includeSurname: true));
        }
    }
}
