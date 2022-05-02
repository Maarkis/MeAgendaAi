using MeAgendaAi.Domains.Validators.ValueObjects;

namespace MeAgendaAi.Domains.ValueObjects;

public class Email : ValueObject
{
	public Email(string address)
	{
		Address = address;
		Validate(this, new EmailValidator());
	}

	public string Address { get; protected set; }

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Address;
	}
}