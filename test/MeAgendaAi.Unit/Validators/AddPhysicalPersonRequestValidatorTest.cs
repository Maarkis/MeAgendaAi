using FluentValidation.TestHelper;
using MeAgendaAi.Application.Validators;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Validators
{
    public class AddPhysicalPersonRequestValidatorTest
    {
        private readonly AddPhysicalPersonRequestValidator Validator;

        private const string ErrorMessageEmpty = "Can't be empty";
        private const string ErrorMessageNull = "Can't be null";

        public AddPhysicalPersonRequestValidatorTest() => Validator = new AddPhysicalPersonRequestValidator();

        [Test]
        public void AddPhysicalPersonValidator_ShouldValidateAndReturnValidAndWithoutError()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();

            var result = Validator.TestValidate(request);

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
                .WithCPFInvalid()
                .WithRGInvalid()
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result.ShouldHaveAnyValidationError();
        }

        [TestCase("", ErrorMessageEmpty)]
        [TestCase(null, ErrorMessageNull)]
        public void AddCompanyRequestValidator_ShouldValidateRequestWithNameFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder()
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
            var requestInvalid = new AddPhysicalPersonRequestBuilder()
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
            var requestInvalid = new AddPhysicalPersonRequestBuilder()
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
            var requestInvalid = new AddPhysicalPersonRequestBuilder()
                .WithConfirmPassword(confirmPassword: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.ConfirmPassword)
                .WithErrorMessage(errorMessage);
        }

        [TestCase("", ErrorMessageEmpty)]
        [TestCase(null, ErrorMessageNull)]
        public void AddCompanyRequestValidator_ShouldValidateRequestWithRGFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder()
                .WithRGInvalid(rg: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.RG)
                .WithErrorMessage(errorMessage);
        }

        [TestCase("", ErrorMessageEmpty)]
        [TestCase(null, ErrorMessageNull)]
        public void AddCompanyRequestValidator_ShouldValidateRequestWithCPFFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder()
                .WithCPFInvalid(cpf: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.CPF)
                .WithErrorMessage(errorMessage);
        }
    }
}