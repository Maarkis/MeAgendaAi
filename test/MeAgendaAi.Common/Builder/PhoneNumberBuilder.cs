using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Enums;

namespace MeAgendaAi.Common.Builder;

public sealed class PhoneNumberBuilder : BaseBuilderEntity<PhoneNumber>
{
	public PhoneNumberBuilder()
	{
		RuleFor(prop => prop.CountryCode, faker => faker.Random.Int(001, 999));
		RuleFor(prop => prop.DialCode, faker => faker.Random.Int(01, 99));
		RuleFor(prop => prop.Number, faker =>
			faker.Random.String2(
				faker.Random.Int(8, 9), "0123456789"));
		RuleFor(prop => prop.Type, faker => faker.PickRandom<EPhoneNumberType>());
		RuleFor(prop => prop.Contact, () =>
			new NameObjectBuilder().WithoutSurname().Generate(false));
	}
}

public static class PhoneNumberBuilderExtensions
{
	public static PhoneNumberBuilder WithDialCode(this PhoneNumberBuilder builder, int dialCode)
	{
		builder.RuleFor(prop => prop.DialCode, () => dialCode);
		return builder;
	}

	public static PhoneNumberBuilder WithCountryCode(this PhoneNumberBuilder builder, int countryCode)
	{
		builder.RuleFor(prop => prop.CountryCode, () => countryCode);
		return builder;
	}

	public static PhoneNumberBuilder WithNumber(this PhoneNumberBuilder builder, string number)
	{
		builder.RuleFor(prop => prop.Number, () => number);
		return builder;
	}

	public static PhoneNumberBuilder WithType(this PhoneNumberBuilder builder, EPhoneNumberType type)
	{
		builder.RuleFor(prop => prop.Type, () => type);
		return builder;
	}

	public static PhoneNumberBuilder WithContact(this PhoneNumberBuilder builder, string name)
	{
		builder.RuleFor(prop => prop.Contact,
			() => new NameObjectBuilder()
				.WithName(name)
				.WithoutSurname()
				.Generate(false));
		return builder;
	}
}