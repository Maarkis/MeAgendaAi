using MeAgendaAi.Domains.Validators.ValueObjects;

namespace MeAgendaAi.Domains.ValueObjects
{
    public class Email : ValueObject
    {        
        public string Address { get; protected set; }

        public Email(string address) : base()
        {
            Address = address;
            Validate(this, new EmailValidator());
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Address;
        }
    }
}