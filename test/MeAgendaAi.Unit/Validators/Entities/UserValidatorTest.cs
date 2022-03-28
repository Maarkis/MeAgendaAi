using FluentValidation.TestHelper;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Validators;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Validators.Entities
{
    public class UserValidatorTest
    {
        private readonly UserValidator<User> Validator;

        public UserValidatorTest() => Validator = new UserValidator<User>(includeSurname: true);

        [Test]
        public void UserValidator_ShouldValidateAndReturnValidAndWithoutError()
        {
            var request = new UserBuilder().Generate();

            var result = Validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void UserValidator_ShouldValidateRequestAndReturnThatItIsInvalidAndInError()
        {
            var requestInvalid = new UserBuilder()
                .WithNameAndSurnameInvalid()
                .WithEmailInvalid()
                .WithPasswordInvalid()
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result.ShouldHaveAnyValidationError();
        }

        [TestCase(0, "Name cannot be empty")]
        [TestCase(2, "Name must contain at least 3 characters")]
        [TestCase(61, "Name must contain a maximum of 60 characters")]
        public void UserValidator_ShouldValidateRequestWithNameFieldInvalidAndReturnError(int fieldLenght, string errorMessage)
        {
            var requestInvalid = new UserBuilder()
                .WithNameInvalidByLength(length: fieldLenght)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Name.Name)
                .WithErrorMessage(errorMessage);
        }

        [TestCase(0, "Surname cannot be empty")]
        [TestCase(2, "Surname must contain at least 3 characters")]
        [TestCase(81, "Surname must contain a maximum of 80 characters")]
        public void UserValidator_ShouldValidateRequestWithSurnameFieldInvalidAndReturnError(int fieldLenght, string errorMessage)
        {
            var requestInvalid = new UserBuilder()
                .WithSurnameInvalidByLength(length: fieldLenght)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Name.Surname)
                .WithErrorMessage(errorMessage);
        }

        [TestCase("", "E-mail cannot be empty")]
        [TestCase("any-email", "Invalid e-mail")]
        public void UserValidator_ShouldValidateRequestWithEmailFieldInvalidAndReturnError(string fieldContent, string errorMessage)
        {
            var requestInvalid = new UserBuilder()
                .WithEmailInvalid(email: fieldContent)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Email.Email)
                .WithErrorMessage(errorMessage);
        }

        [TestCase(0, "Password cannot be empty")]
        [TestCase(5, "Password must contain at least 6 characters")]
        [TestCase(33, "Password must contain a maximum of 32 characters")]
        public void UserValidator_ShouldValidateRequestWithPasswordFieldInvalidAndReturnError(int fieldLenght, string errorMessage)
        {
            var requestInvalid = new UserBuilder()
                .WithPasswordInvalidByLength(length: fieldLenght)
                .Generate();

            var result = Validator.TestValidate(requestInvalid);

            result
                .ShouldHaveValidationErrorFor(field => field.Password)
                .WithErrorMessage(errorMessage);
        }
    }
}