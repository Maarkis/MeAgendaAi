using FluentValidation.TestHelper;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Validators;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Validators.Entities
{
    public class CompanyValidatorTest
    {
        private readonly CompanyValidator Validator;

        public CompanyValidatorTest() => Validator = new CompanyValidator();

        [Test]
        public void CompanyValidator_ShouldValidateAndReturnValidAndWithoutError()
        {
            var request = new CompanyBuilder().Generate();

            var result = Validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void CompanyValidator_ShouldValidateRequestAndReturnThatItIsInvalidAndInError()
        {
            var requestInvalid = new CompanyBuilder()
                .WithNameInvalid()
                .WithEmailInvalid()
                .WithPasswordInvalid()
                .WithCNPJInvalid()
                .WithDescriptionInvalid()
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result.ShouldHaveAnyValidationError();
        }

        [TestCase(0, "Name cannot be empty")]
        [TestCase(2, "Name must contain at least 3 characters")]
        [TestCase(61, "Name must contain a maximum of 60 characters")]
        public void CompanyValidator_ShouldValidateRequestWithNameFieldInvalidAndReturnError(int fieldLenght, string errorMessage)
        {
            var requestInvalid = new CompanyBuilder()
                .WithNameInvalidByLength(length: fieldLenght)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Name.FirstName)
                .WithErrorMessage(errorMessage);
        }

        [TestCase("", "E-mail cannot be empty")]
        [TestCase("any-email", "Invalid e-mail")]
        public void CompanyValidator_ShouldValidateRequestWithEmailFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new CompanyBuilder()
                .WithEmailInvalid(email: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Email.Address)
                .WithErrorMessage(errorMessage);
        }

        [TestCase(0, "Password cannot be empty")]
        [TestCase(5, "Password must contain at least 6 characters")]
        [TestCase(33, "Password must contain a maximum of 32 characters")]
        public void CompanyValidator_ShouldValidateRequestWithPasswordFieldInvalidAndReturnError(int fieldLenght, string errorMessage)
        {
            var requestInvalid = new CompanyBuilder()
                .WithPasswordInvalidByLength(length: fieldLenght)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Password)
                .WithErrorMessage(errorMessage);
        }

        [TestCase("", "CPNJ cannot be empty")]
        [TestCase(null, "CPNJ cannot be null")]
        public void CompanyValidator_ShouldValidateRequestWithCNPJFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new CompanyBuilder()
                .WithCNPJInvalid(cnpj: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.CNPJ)
                .WithErrorMessage(errorMessage);
        }

        [Test]
        public void CompanyValidator_ShouldValidateRequestWithLimitCancelHoursieldInvalidAndReturnError()
        {
            var errorMessage = "Limit cancel hours cannot be empty";
            var limitCancelHours = 0;
            var requestInvalid = new CompanyBuilder()
                .WithLimitCancelHours(limitCancelHours)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.LimitCancelHours)
                .WithErrorMessage(errorMessage);
        }

        [TestCase(2, "Description must contain at least 3 characters")]
        [TestCase(161, "Description must contain a maximum of 160 characters")]
        public void CompanyValidator_ShouldValidateRequestWithDescriptionFieldInvalidAndReturnError(int fieldLenght, string errorMessage)
        {
            var requestInvalid = new CompanyBuilder()
                .WithDescriptionInvalidByLength(length: fieldLenght)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Description)
                .WithErrorMessage(errorMessage);
        }

        [TestCase("", "Description cannot be empty")]
        [TestCase(null, "Description cannot be null")]
        public void CompanyValidator_ShouldValidateRequestWithDescriptionFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new CompanyBuilder()
                .WithDescriptionInvalid(description: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Description)
                .WithErrorMessage(errorMessage);
        }
    }
}