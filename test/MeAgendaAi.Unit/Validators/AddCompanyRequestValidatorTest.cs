using FluentValidation.TestHelper;
using MeAgendaAi.Application.Validators;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Validators
{
    public class AddCompanyRequestValidatorTest
    {
        private readonly AddCompanyRequestValidator Validator;

        private const string ErrorMessageEmpty = "Can't be empty";
        private const string ErrorMessageNull = "Can't be null";

        public AddCompanyRequestValidatorTest() => Validator = new AddCompanyRequestValidator();

        [Test]
        public void AddCompanyRequestValidator_ShouldValidateAndReturnValidAndWithoutError()
        {
            var request = new AddCompanyRequestBuilder().Generate();

            var result = Validator.TestValidate(request);

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
                .WithCNPJInvalid()
                .WithDescriptionInvalid()
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result.ShouldHaveAnyValidationError();
        }

        [TestCase("", ErrorMessageEmpty)]
        [TestCase(null, ErrorMessageNull)]
        public void AddCompanyRequestValidator_ShouldValidateRequestWithNameFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new AddCompanyRequestBuilder()
                .WithNameInvalid(name: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Name)
                .WithErrorMessage(errorMessage);
        }

        [TestCase("", ErrorMessageEmpty)]
        [TestCase(null, ErrorMessageNull)]
        public void AddCompanyRequestValidator_ShouldValidateRequestWithEmailFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new AddCompanyRequestBuilder()
                .WithEmailInvalid(email: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Email)
                .WithErrorMessage(errorMessage);
        }

        [TestCase("", ErrorMessageEmpty)]
        [TestCase(null, ErrorMessageNull)]
        public void AddCompanyRequestValidator_ShouldValidateRequestWithPasswordFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new AddCompanyRequestBuilder()
                .WithPasswordInvalid(password: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Password)
                .WithErrorMessage(errorMessage);
        }

        [TestCase("", ErrorMessageEmpty)]
        [TestCase(null, ErrorMessageNull)]
        public void AddCompanyRequestValidator_ShouldValidateRequestWithConfirmPasswordFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new AddCompanyRequestBuilder()
                .WithConfirmPassword(confirmPassword: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.ConfirmPassword)
                .WithErrorMessage(errorMessage);
        }

        [TestCase("", ErrorMessageEmpty)]
        [TestCase(null, ErrorMessageNull)]
        public void AddCompanyRequestValidator_ShouldValidateRequestWithCNPJFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new AddCompanyRequestBuilder()
                .WithCNPJInvalid(cnpj: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.CNPJ)
                .WithErrorMessage(errorMessage);
        }

        [TestCase("", ErrorMessageEmpty)]
        [TestCase(null, ErrorMessageNull)]
        public void AddCompanyRequestValidator_ShouldValidateRequestWithDescriptionFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new AddCompanyRequestBuilder()
                .WithDescriptionInvalid(description: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Description)
                .WithErrorMessage(errorMessage);
        }

        [TestCase(0, ErrorMessageEmpty)]
        public void AddCompanyRequestValidator_ShouldValidateRequestWithLimitCancelHoursFieldInvalidAndReturnError(int fieldContent, string errorMessage)
        {
            var requestInvalid = new AddCompanyRequestBuilder()
                .WithLimitCancelHours(limitCancelHours: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.LimitCancelHours)
                .WithErrorMessage(errorMessage);
        }
    }
}