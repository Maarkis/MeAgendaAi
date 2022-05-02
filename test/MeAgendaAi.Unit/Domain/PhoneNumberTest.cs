using System;
using Bogus;
using FluentAssertions;
using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Enums;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Domain;

public class PhoneNumberTest
{
	private readonly Faker _faker;

	public PhoneNumberTest()
	{
		_faker = new Faker("pt_BR");
	}

	[Test]
	public void ShouldCreatedAnInstanceValidOfTypePhoneNumber()
	{
		var countryCode = _faker.Random.Int(min: 001, max: 999);
		var dialCode = _faker.Random.Int(min: 01, max: 99);
		var number = _faker.Random.String2(_faker.Random.Int(min: 8, max: 9), "0123456789");
		var type = _faker.PickRandom<EPhoneNumberType>();
		var contact = _faker.Name.FirstName();

		var phone = new PhoneNumber(countryCode, dialCode, number, type, contact);

		phone.Valid.Should().BeTrue();
		phone.ValidationResult.Errors.Should().BeEmpty();
	}

	[Test]
	public void ShouldCreatedAnInstanceValidOfTypePhoneNumberWithCorrectValues()
	{
		var phoneNumberExpected = new
		{
			CountryCode = _faker.Random.Int(min: 001, max: 999),
			DialCode = _faker.Random.Int(min: 01, max: 99),
			Number = _faker.Random.String2(_faker.Random.Int(min: 8, max: 9), "0123456789"),
			Type = _faker.PickRandom<EPhoneNumberType>(),
			Contact = new NameObjectBuilder().WithoutSurname().Generate()
		};

		var phone = new PhoneNumber(
			phoneNumberExpected.CountryCode, phoneNumberExpected.DialCode, phoneNumberExpected.Number,
			phoneNumberExpected.Type, phoneNumberExpected.Contact.FirstName);

		phone.Should().BeEquivalentTo(phoneNumberExpected, options => options.ExcludingMissingMembers());
	}

	[Test]
	public void ShouldCreatedAnInstanceInvalidOfTypePhoneNumber()
	{
		const int countryCode = 0;
		const int dialCode = 0;
		const string number = "";
		const EPhoneNumberType type = (EPhoneNumberType)4;
		const string contact = "";

		var phone = new PhoneNumber(countryCode, dialCode, number, type, contact);

		phone.Valid.Should().BeFalse();
		phone.ValidationResult.Errors.Should().NotBeEmpty();
	}

	[Test]
	public void ShouldCreateAValidInstanceOfTypePhoneNumberWithTypeOtherWhenNotDeclaringPhoneType()
	{
		var countryCode = _faker.Random.Int(min: 001, max: 999);
		var dialCode = _faker.Random.Int(min: 01, max: 99);
		var number = _faker.Random.String2(_faker.Random.Int(min: 8, max: 9), "0123456789");

		var phone = new PhoneNumber(countryCode, dialCode, number);

		phone.Type.Should().Be(EPhoneNumberType.OtherPhone);
	}

	[Test]
	public void ShouldCreateAValidInstanceOfTypePhoneNumberWithNullContactWhenNotPassingContactName()
	{
		var countryCode = _faker.Random.Int(min: 001, max: 999);
		var dialCode = _faker.Random.Int(min: 01, max: 99);
		var number = _faker.Random.String2(_faker.Random.Int(min: 8, max: 9), "0123456789");

		var phone = new PhoneNumber(countryCode, dialCode, number);

		phone.Contact.Should().BeNull();
	}

	[TestCase(EPhoneNumberType.HomePhone)]
	[TestCase(EPhoneNumberType.MobilePhone)]
	[TestCase(EPhoneNumberType.OtherPhone)]
	public void ShouldReturnFullPhoneNumberCorrectlyRegardlessOfType(EPhoneNumberType type)
	{
		var countryCode = _faker.Random.Int(min: 001, max: 999);
		var dialCode = _faker.Random.Int(min: 01, max: 99);
		var number = _faker.Random.String2(_faker.Random.Int(min: 8, max: 9), "0123456789");
		var fullNumberExpected = $"{countryCode}{dialCode}{number}";

		var phone = new PhoneNumber(countryCode, dialCode, number, type);

		phone.FullPhoneNumber().Should().Be(fullNumberExpected);
	}

	[Test]
	public void ShouldReturnFullPhoneNumberFormattedCorrectlyWhenPhoneTypeIsOther()
	{
		var countryCode = _faker.Random.Int(min: 001, max: 999);
		var dialCode = _faker.Random.Int(min: 01, max: 99);
		var number = _faker.Random.String2(_faker.Random.Int(min: 8, max: 9), "0123456789");
		var fullNumberExpected = $"+{countryCode} {dialCode} {number}";

		var phone = new PhoneNumber(countryCode, dialCode, number, EPhoneNumberType.OtherPhone);

		phone.FullPhoneNumberFormatted()
			.Should()
			.Be(fullNumberExpected);
	}

	[Test]
	public void ShouldReturnFullPhoneNumberFormattedCorrectlyWhenPhoneTypeIsMobile()
	{
		var countryCode = _faker.Random.Int(min: 001, max: 999);
		var dialCode = _faker.Random.Int(min: 01, max: 99);
		var number = _faker.Random.String2(_faker.Random.Int(min: 9, max: 9), "0123456789");
		var firstPartOfNumber = number[..^4];
		var lastPartOfNumber = number[^4..];
		var fullNumberFormattedExpected = $"+{countryCode} {dialCode} {firstPartOfNumber}-{lastPartOfNumber}";

		var phone = new PhoneNumber(countryCode, dialCode, number, EPhoneNumberType.MobilePhone);

		phone.FullPhoneNumberFormatted()
			.Should()
			.Be(fullNumberFormattedExpected);
	}

	[Test]
	public void ShouldReturnFullPhoneNumberFormattedCorrectlyWhenPhoneTypeIsHome()
	{
		var countryCode = _faker.Random.Int(min: 001, max: 999);
		var dialCode = _faker.Random.Int(min: 01, max: 99);
		var number = _faker.Random.String2(_faker.Random.Int(min: 8, max: 8), "0123456789");
		var firstPartOfNumber = number[..^4];
		var lastPartOfNumber = number[^4..];
		var fullNumberFormattedExpected = $"+{countryCode} {dialCode} {firstPartOfNumber}-{lastPartOfNumber}";

		var phone = new PhoneNumber(countryCode, dialCode, number, EPhoneNumberType.HomePhone);

		phone.FullPhoneNumberFormatted()
			.Should()
			.Be(fullNumberFormattedExpected);
	}

	[TestCase("", EPhoneNumberType.MobilePhone)]
	[TestCase(null, EPhoneNumberType.HomePhone)]
	public void ShouldFullPhoneNumberFormattedReturnExceptionWhenNumberIsEmptyOrNullAndRegardlessOfPhoneType(
		string number,
		EPhoneNumberType type)
	{
		var countryCode = _faker.Random.Int(min: 001, max: 999);
		var dialCode = _faker.Random.Int(min: 01, max: 99);

		var phone = new PhoneNumber(countryCode, dialCode, number, type);

		var functionWithException = () => phone.FullPhoneNumberFormatted();

		functionWithException
			.Should()
			.ThrowExactly<NullReferenceException>()
			.WithMessage(nameof(phone.Number));
	}

	[TestCase(-1)]
	[TestCase(0)]
	[TestCase(4)]
	public void ShouldFullPhoneNumberFormattedReturnExceptionWhenPhoneTypeIsInvalid(EPhoneNumberType typeInvalid)
	{
		var countryCode = _faker.Random.Int(min: 001, max: 999);
		var dialCode = _faker.Random.Int(min: 01, max: 99);
		var number = _faker.Random.String2(_faker.Random.Int(min: 8, max: 8), "0123456789");
		var phone = new PhoneNumber(countryCode, dialCode, number, typeInvalid);

		var functionWithException = () => phone.FullPhoneNumberFormatted();

		functionWithException
			.Should()
			.ThrowExactly<ArgumentOutOfRangeException>();
	}
}