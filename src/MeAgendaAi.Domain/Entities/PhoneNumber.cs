using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Domains.Enums;
using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;
using MeAgendaAi.Infra.Extension;

namespace MeAgendaAi.Domains.Entities;

public class PhoneNumber : Entity
{
	private const string MobileFormat = @"{0:00000-0000}";
	private const string HomeFormat = @"{0:0000-0000}";

	protected PhoneNumber()
	{
	}

	public PhoneNumber(int countryCode, int dialCode, string number)
	{
		CountryCode = countryCode;
		DialCode = dialCode;
		Number = number.OnlyNumbers();
		Type = EPhoneNumberType.OtherPhone;

		Validate();
	}

	public PhoneNumber(
		int countryCode, int dialCode, string number, EPhoneNumberType type)
	{
		CountryCode = countryCode;
		DialCode = dialCode;
		Number = number.OnlyNumbers();
		Type = type;

		Validate();
	}

	public PhoneNumber(int countryCode, int dialCode, string number, EPhoneNumberType type, string? contact)
	{
		Number = number.OnlyNumbers();
		CountryCode = countryCode;
		DialCode = dialCode;
		Type = type;

		if (contact is not null)
			Contact = new Name(contact);

		Validate();
	}

	public Name? Contact { get; protected set; }
	public int CountryCode { get; protected set; }
	public int DialCode { get; protected set; }
	public string Number { get; protected set; } = string.Empty;
	public EPhoneNumberType Type { get; protected set; }

	public virtual Guid UserId { get; protected set; }
	public virtual User User { get; set; } = default!;

	public string FullPhoneNumberFormatted() => $"+{CountryCode} {DialCode} {Formatted()}";
	public string FullPhoneNumber() => $"{CountryCode}{DialCode}{Number}";

	private string Formatted()
	{
		if (string.IsNullOrEmpty(Number))
			throw new NullReferenceException(nameof(Number));

		return Type switch
		{
			EPhoneNumberType.HomePhone => string.Format(HomeFormat, Convert.ToUInt64(Number)),
			EPhoneNumberType.MobilePhone => string.Format(MobileFormat, Convert.ToUInt64(Number)),
			EPhoneNumberType.OtherPhone => Number,
			_ => throw new ArgumentOutOfRangeException(nameof(Type))
		};
	}

	public bool Validate() => Validate(this, new PhoneNumberValidator());
}