using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Enums;

namespace MeAgendaAi.Domains.RequestAndResponse;

public class PhoneRequest
{
	public PhoneRequest(string phone, int countryCode, int dialCode, EPhoneNumberType type, string? contact)
	{
		Phone = phone;
		CountryCode = countryCode;
		DialCode = dialCode;
		Type = type;
		Contact = contact;
	}

	public string Phone { get; init; }
	public int CountryCode { get; init; }
	public int DialCode { get; init; }
	public EPhoneNumberType Type { get; init; }
	public string? Contact { get; init; }

	public PhoneNumber ToPhoneNumber() =>
		string.IsNullOrWhiteSpace(Contact)
			? new PhoneNumber(CountryCode, DialCode, Phone, Type)
			: new PhoneNumber(CountryCode, DialCode, Phone, Type, Contact);
}

public static class PhoneRequestExtensions
{
	public static IEnumerable<PhoneNumber> ToPhoneNumbers(this IEnumerable<PhoneRequest> phoneRequests) =>
		phoneRequests.Select(phone => phone.ToPhoneNumber());
}