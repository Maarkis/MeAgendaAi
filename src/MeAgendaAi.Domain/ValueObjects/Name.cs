using MeAgendaAi.Domains.Validators.ValueObjects;

namespace MeAgendaAi.Domains.ValueObjects
{
    public class Name : ValueObject
    {
        public string FirstName { get; protected set; } = default!;

        public string Surname { get; protected set; } = default!;

        public string FullName => $"{FirstName} {Surname}".Trim();

        protected Name()
        {
        }

        public Name(string firstName, string lastName) : base()
        {
            FirstName = firstName;
            Surname = lastName;

            Validate(this, new NameValidator(includeSurname: true));
        }

        public Name(string firstName) : base()
        {
            FirstName = firstName;
            Surname = string.Empty;
            Validate(this, new NameValidator(includeSurname: false));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return Surname;
            yield return FullName;
        }
    }
}