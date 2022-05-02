using FluentValidation.TestHelper;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Validators;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Validators.Entities;

public class PhysicalPersonValidatorTest
{
	private readonly PhysicalPersonValidator _validator;

	public PhysicalPersonValidatorTest()
	{
		_validator = new PhysicalPersonValidator();
	}

	[Test]
	public void PhysicalPersonValidator_ShouldValidateAndReturnValidAndWithoutError()
	{
		var request = new PhysicalPersonBuilder().Generate();

		var result = _validator.TestValidate(request);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Test]
	public void PhysicalPersonValidator_ShouldValidateRequestAndReturnThatItIsInvalidAndInError()
	{
		var requestInvalid = new PhysicalPersonBuilder()
			.WithNameInvalid()
			.WithEmailInvalid()
			.WithPasswordInvalid()
			.WithCpfInvalid()
			.WithRgInvalid()
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result.ShouldHaveAnyValidationError();
	}

	[TestCase(0, "Name cannot be empty")]
	[TestCase(2, "Name must contain at least 3 characters")]
	[TestCase(61, "Name must contain a maximum of 60 characters")]
	public void PhysicalPersonValidator_ShouldValidateRequestWithNameFieldInvalidAndReturnError(int fieldLenght,
		string errorMessage)
	{
		var requestInvalid = new PhysicalPersonBuilder()
			.WithNameInvalidByLength(fieldLenght)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Name.FirstName)
			.WithErrorMessage(errorMessage);
	}

	[TestCase(0, "Surname cannot be empty")]
	[TestCase(2, "Surname must contain at least 3 characters")]
	[TestCase(81, "Surname must contain a maximum of 80 characters")]
	public void PhysicalPersonValidator_ShouldValidateRequestWithSurnameFieldInvalidAndReturnError(int fieldLenght,
		string errorMessage)
	{
		var requestInvalid = new PhysicalPersonBuilder()
			.WithSurnameInvalidByLength(fieldLenght)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Name.Surname)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", "E-mail cannot be empty")]
	[TestCase("any-email", "Invalid e-mail")]
	public void PhysicalPersonValidator_ShouldValidateRequestWithEmailFieldInvalidAndReturnError(string fieldContent,
		string errorMessage)
	{
		var requestInvalid = new PhysicalPersonBuilder()
			.WithEmailInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Email.Address)
			.WithErrorMessage(errorMessage);
	}

	[TestCase(0, "Password cannot be empty")]
	[TestCase(5, "Password must contain at least 6 characters")]
	[TestCase(33, "Password must contain a maximum of 32 characters")]
	public void PhysicalPersonValidator_ShouldValidateRequestWithPasswordFieldInvalidAndReturnError(int fieldLenght,
		string errorMessage)
	{
		var requestInvalid = new PhysicalPersonBuilder()
			.WithPasswordInvalidByLength(fieldLenght)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Password)
			.WithErrorMessage(errorMessage);
	}

	[TestCase(null, "CPF cannot be null")]
	[TestCase("", "CPF cannot be empty")]
	public void PhysicalPersonValidator_ShouldValidateRequestWithCPFFieldInvalidAndReturnError(string fieldContent,
		string errorMessage)
	{
		var requestInvalid = new PhysicalPersonBuilder()
			.WithCpfInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.CPF)
			.WithErrorMessage(errorMessage);
	}

	[TestCase(null, "RG cannot be null")]
	[TestCase("", "RG cannot be empty")]
	public void PhysicalPersonValidator_ShouldValidateRequestWithRGFieldInvalidAndReturnError(string fieldContent,
		string errorMessage)
	{
		var requestInvalid = new PhysicalPersonBuilder()
			.WithRgInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.RG)
			.WithErrorMessage(errorMessage);
	}
}