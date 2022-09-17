using System.Collections.Generic;
using FluentValidation.TestHelper;
using MeAgendaAi.Application.Validators;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.RequestAndResponse;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Validators;

public class AddCompanyRequestValidatorTest
{
	private const string ErrorMessageEmpty = "Can't be empty";
	private const string ErrorMessageNull = "Can't be null";
	private readonly AddCompanyRequestValidator _validator;

	public AddCompanyRequestValidatorTest()
	{
		_validator = new AddCompanyRequestValidator();
	}

	[Test]
	public void AddCompanyRequestValidator_ShouldValidateAndReturnValidAndWithoutError()
	{
		var request = new AddCompanyRequestBuilder().Generate();

		var result = _validator.TestValidate(request);

		result.ShouldNotHaveAnyValidationErrors();
	}

	[Test]
	public void AddCompanyRequestValidator_ShouldValidateRequestAndReturnThatItIsInvalidAndInError()
	{
		var requestInvalid = new AddCompanyRequestBuilder()
			.WithNameInvalid()
			.WithEmailInvalid()
			.WithConfirmPasswordInvalid()
			.WithPasswordInvalid()
			.WithCnpjInvalid()
			.WithDescriptionInvalid()
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result.ShouldHaveAnyValidationError();
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithNameFieldInvalidAndReturnError(string fieldContent,
		string errorMessage)
	{
		var requestInvalid = new AddCompanyRequestBuilder()
			.WithNameInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Name)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithEmailFieldInvalidAndReturnError(string fieldContent,
		string errorMessage)
	{
		var requestInvalid = new AddCompanyRequestBuilder()
			.WithEmailInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Email)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithPasswordFieldInvalidAndReturnError(
		string fieldContent, string errorMessage)
	{
		var requestInvalid = new AddCompanyRequestBuilder()
			.WithPasswordInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Password)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithConfirmPasswordFieldInvalidAndReturnError(
		string fieldContent, string errorMessage)
	{
		var requestInvalid = new AddCompanyRequestBuilder()
			.WithConfirmPassword(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.ConfirmPassword)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithCNPJFieldInvalidAndReturnError(string fieldContent,
		string errorMessage)
	{
		var requestInvalid = new AddCompanyRequestBuilder()
			.WithCnpjInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.CNPJ)
			.WithErrorMessage(errorMessage);
	}

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithDescriptionFieldInvalidAndReturnError(
		string fieldContent, string errorMessage)
	{
		var requestInvalid = new AddCompanyRequestBuilder()
			.WithDescriptionInvalid(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Description)
			.WithErrorMessage(errorMessage);
	}

	[TestCase(0, ErrorMessageEmpty)]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithLimitCancelHoursFieldInvalidAndReturnError(
		int fieldContent, string errorMessage)
	{
		var requestInvalid = new AddCompanyRequestBuilder()
			.WithLimitCancelHours(fieldContent)
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.LimitCancelHours)
			.WithErrorMessage(errorMessage);
	}

	[Test]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithPhonesNullFieldAndReturnError()
	{
		var requestInvalid = new AddCompanyRequestBuilder()
			.WithPhonesInvalid()
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Phones)
			.WithErrorMessage(ErrorMessageNull);
	}

	[Test]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithPhonesEmptyListFieldAndReturnError()
	{
		var requestInvalid = new AddCompanyRequestBuilder()
			.WithPhonesInvalid(new List<PhoneRequest>())
			.Generate();

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Phones)
			.WithErrorMessage(ErrorMessageEmpty);
	}
}