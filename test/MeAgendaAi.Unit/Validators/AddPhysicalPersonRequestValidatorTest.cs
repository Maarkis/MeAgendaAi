using System.Collections.Generic;
using FluentValidation.TestHelper;
using MeAgendaAi.Application.Validators;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.RequestAndResponse;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Validators;

public class AddPhysicalPersonRequestValidatorTest
{
	private const string ErrorMessageEmpty = "Can't be empty";
	private const string ErrorMessageNull = "Can't be null";
	private readonly AddPhysicalPersonRequestValidator _validator;

	public AddPhysicalPersonRequestValidatorTest() => _validator = new AddPhysicalPersonRequestValidator();

	[Test]
	public void AddPhysicalPersonValidator_ShouldValidateAndReturnValidAndWithoutError()
	{
		var request = new AddPhysicalPersonRequestBuilder().Generate();

		var result = _validator.TestValidate(request);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Test]
	public void AddPhysicalPersonValidator_ShouldValidateRequestAndReturnThatItIsInvalidAndInError()
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder()
			.WithNameInvalid()
			.WithEmailInvalid()
			.WithSurnameInvalid()
			.WithConfirmPasswordInvalid()
			.WithPasswordInvalid()
			.WithCpfInvalid()
			.WithRgInvalid()
			.WithPhonesInvalid()
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result.ShouldHaveAnyValidationError();
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddPhysicalPersonRequestValidator_ShouldValidateRequestWithNameFieldInvalidAndReturnError(string fieldContent,
		string errorMessage)
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder()
			.WithNameInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Name)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddPhysicalPersonRequestValidator_ShouldValidateRequestWithEmailFieldInvalidAndReturnError(string fieldContent,
		string errorMessage)
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder()
			.WithEmailInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Email)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddPhysicalPersonRequestValidator_ShouldValidateRequestWithPasswordFieldInvalidAndReturnError(
		string fieldContent, string errorMessage)
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder()
			.WithPasswordInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Password)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddPhysicalPersonRequestValidator_ShouldValidateRequestWithConfirmPasswordFieldInvalidAndReturnError(
		string fieldContent, string errorMessage)
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder()
			.WithConfirmPassword(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.ConfirmPassword)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddPhysicalPersonRequestValidator_ShouldValidateRequestWithRGFieldInvalidAndReturnError(string fieldContent,
		string errorMessage)
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder()
			.WithRgInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.RG)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddPhysicalPersonRequestValidator_ShouldValidateRequestWithCPFFieldInvalidAndReturnError(string fieldContent,
		string errorMessage)
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder()
			.WithCpfInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.CPF)
			.WithErrorMessage(errorMessage);
	}
	
	
	[Test]
	public void AddPhysicalPersonRequestValidator_ShouldValidateRequestWithPhonesNullFieldAndReturnError()
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder()
			.WithPhonesInvalid()
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Phones)
			.WithErrorMessage(ErrorMessageNull);
	}
	
	[Test]
	public void AddPhysicalPersonRequestValidator_ShouldValidateRequestWithPhonesEmptyListFieldAndReturnError()
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder()
			.WithPhonesInvalid(new List<PhoneRequest>())
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Phones)
			.WithErrorMessage(ErrorMessageEmpty);
	}
}