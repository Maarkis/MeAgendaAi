using FluentValidation.TestHelper;
using MeAgendaAi.Application.Validators;
using MeAgendaAi.Domains.RequestAndResponse;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Validators;

public class RequestPasswordValidatorTest
{
	private readonly ResetPasswordRequestValidator _validator;
	
	private const string ErrorMessageEmpty = "Can't be empty";
	private const string ErrorMessageNull = "Can't be null";

	public RequestPasswordValidatorTest() =>  _validator = new ResetPasswordRequestValidator();

	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithTokenFieldInvalidAndReturnError(string fieldContent, string errorMessage)
	{
		var requestInvalid = new ResetPasswordRequest()
		{
			Token = fieldContent,
			Password = "any-password",
			ConfirmPassword = "any-confirm-password"
		};

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Token)
			.WithErrorMessage(errorMessage);
	}
	
	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithPasswordFieldInvalidAndReturnError(string fieldContent, string errorMessage)
	{
		var requestInvalid = new ResetPasswordRequest()
		{
			Token = "Token",
			Password = fieldContent,
			ConfirmPassword = "any-confirm-password"
		};

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.Password)
			.WithErrorMessage(errorMessage);
	}
	
	[TestCase("", ErrorMessageEmpty)]
	[TestCase(null, ErrorMessageNull)]
	public void AddCompanyRequestValidator_ShouldValidateRequestWithConfirmPasswordFieldInvalidAndReturnError(string fieldContent, string errorMessage)
	{
		var requestInvalid = new ResetPasswordRequest()
		{
			Token = "Token",
			Password = "any-password",
			ConfirmPassword = fieldContent
		};

		var result = _validator.TestValidate(requestInvalid);

		result
			.ShouldHaveValidationErrorFor(field => field.ConfirmPassword)
			.WithErrorMessage(errorMessage);
	}
}