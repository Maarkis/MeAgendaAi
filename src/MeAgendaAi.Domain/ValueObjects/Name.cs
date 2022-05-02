using MeAgendaAi.Domains.Validators.ValueObjects;

namespace MeAgendaAi.Domains.ValueObjects;

public class Name : ValueObject
{
	protected Name()
	{
	}

	public Name(string firstName, string lastName)
	{
		FirstName = firstName;
		Surname = lastName;

		Validate(this, new NameValidator(true));
	}

	public Name(string firstName)
	{
		FirstName = firstName;
		Surname = string.Empty;
		Validate(this, new NameValidator(false));
	}

	public string FirstName { get; protected set; } = default!;

	public string Surname { get; protected set; } = default!;

	public string FullName => $"{FirstName} {Surname}".Trim();

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return FirstName;
		yield return Surname;
		yield return FullName;
	}
}