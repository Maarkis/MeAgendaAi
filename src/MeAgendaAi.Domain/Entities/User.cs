using System.Collections.ObjectModel;
using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;
using MeAgendaAi.Infra.Extension;
using Newtonsoft.Json;

namespace MeAgendaAi.Domains.Entities;

public class User : Entity
{
	public Name Name { get; protected set; } = default!;
	public Email Email { get; protected set; } = default!;
	public string Password { get; protected set; } = default!;
	public bool IsActive { get; protected set; }

	[JsonProperty] // TODO This is a problem?
	protected readonly ICollection<PhoneNumber> _phoneNumbers = default!;

	public IReadOnlyCollection<PhoneNumber> PhoneNumbers =>
		_phoneNumbers.IsEmpty() ? new List<PhoneNumber>() : new List<PhoneNumber>(_phoneNumbers);

	protected User()
	{
	}

	public User(string email, string password, string name, IEnumerable<PhoneNumber> phones)
	{
		Email = new Email(email);
		Password = password;
		Name = new Name(name);
		IsActive = false;
		_phoneNumbers = new Collection<PhoneNumber>(phones.ToList());

		Validate(false);
	}

	public User(string email, string password, string name, string surname, IEnumerable<PhoneNumber> phones)
	{
		Email = new Email(email);
		Password = password;
		Name = new Name(name, surname);
		IsActive = false;
		_phoneNumbers = new Collection<PhoneNumber>(phones.ToList());

		Validate(true);
	}

	public bool Validate(bool includeSurname) =>
		Validate(this, new UserValidator<User>(includeSurname));

	public void Encrypt(string password)
	{
		Password = password;
		UpdatedAt();
	}

	public void Active()
	{
		IsActive = true;
		UpdatedAt();
	}

	public void AddPhoneNumber(PhoneNumber phoneNumber) => _phoneNumbers.Add(phoneNumber);
}