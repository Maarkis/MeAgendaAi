using AutoBogus;
using MeAgendaAi.Domains.Enums;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Common.Builder.RequestAndResponse;

public sealed class PhoneRequestBuilder : AutoFaker<PhoneRequest>
{
	public PhoneRequestBuilder() : base("pt_BR")
	{
		RuleFor(prop => prop.CountryCode, faker => faker.Random.Int(001, 999));
		RuleFor(prop => prop.DialCode, faker => faker.Random.Int(01, 99));
		RuleFor(prop => prop.Phone, faker => faker.Random.String2(faker.Random.Int(8, 9), "0123456789"));
		RuleFor(prop => prop.Type, faker => faker.PickRandom<EPhoneNumberType>());
		RuleFor(prop => prop.Contact, faker => faker.Name.FirstName());
	}
}