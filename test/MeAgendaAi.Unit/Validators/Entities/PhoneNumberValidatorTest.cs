using FluentValidation.TestHelper;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Enums;
using MeAgendaAi.Domains.Validators;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Validators.Entities;

public class PhoneNumberValidatorTest
{
	private readonly PhoneNumberValidator _validator;

	public PhoneNumberValidatorTest()
	{
		_validator = new PhoneNumberValidator();
	}

	[Test]
	public void PhoneNumberValidator_ShouldValidateAndReturnValidAndWithoutError()
	{
		var phoneValid = new PhoneNumberBuilder().Generate();

		var result = _validator.TestValidate(phoneValid);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[TestCase(0, "Country Code can't be empty")]
	public void PhoneNumberValidator_ShouldValidatePhoneNumberWithCountryCodeFieldInvalidAndReturnError(int countryCode,
		string errorMessage)
	{
		var phoneInvalid = new PhoneNumberBuilder().WithCountryCode(countryCode).Generate();

		var result = _validator.TestValidate(phoneInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.CountryCode)
			.WithErrorMessage(errorMessage);
	}

	[TestCase(0, "Dial Code can't be empty")]
	public void PhoneNumberValidator_ShouldValidatePhoneNumberWithDialCodeFieldInvalidAndReturnError(int dialCode,
		string errorMessage)
	{
		var phoneInvalid = new PhoneNumberBuilder().WithDialCode(dialCode).Generate();

		var result = _validator.TestValidate(phoneInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.DialCode)
			.WithErrorMessage(errorMessage);
	}


	[TestCase("", "Number can't be empty")]
	[TestCase(null, "Number can't be null")]
	public void PhoneNumberValidator_ShouldValidatePhoneNumberWithNumberFieldInvalidAndReturnError(string number,
		string errorMessage)
	{
		var phoneInvalid = new PhoneNumberBuilder().WithNumber(number).Generate();

		var result = _validator.TestValidate(phoneInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Number)
			.WithErrorMessage(errorMessage);
	}

	[TestCase(-1, "Phone type entered incorrectly")]
	[TestCase(0, "Phone type entered incorrectly")]
	[TestCase(4, "Phone type entered incorrectly")]
	public void PhoneNumberValidator_ShouldValidatePhoneNumberWithTypeFieldInvalidAndReturnError(int phoneEnumCode,
		string errorMessage)
	{
		var phoneInvalid = new PhoneNumberBuilder().WithType((EPhoneNumberType)phoneEnumCode).Generate();

		var result = _validator.TestValidate(phoneInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Type)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", "Name cannot be empty")]
	[TestCase(null, "Name cannot be empty")]
	public void PhoneNumberValidator_ShouldValidatePhoneNumberWithContactFieldInvalidAndReturnError(string name,
		string errorMessage)
	{
		var phoneInvalid = new PhoneNumberBuilder().WithContact(name).Generate();

		var result = _validator.TestValidate(phoneInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Contact!.FirstName)
			.WithErrorMessage(errorMessage);
	}
}