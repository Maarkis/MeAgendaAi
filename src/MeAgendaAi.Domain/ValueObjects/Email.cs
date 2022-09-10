using MeAgendaAi.Domains.Validators.ValueObjects;
using static System.String;

namespace MeAgendaAi.Domains.ValueObjects;

public class Email : ValueObject
{
	public Email(string address)
	{
		Address = address;
		Validate(this, new EmailValidator());
	}

	public string Address { get; protected set; }
	public string Domain => IsNullOrEmpty(Address) || NotContainAtSign() ? Empty : Address.Split("@")[1];

	private bool NotContainAtSign() => !Address.Contains('@');
}
	