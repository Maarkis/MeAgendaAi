using Bogus;
using FluentAssertions;
using MeAgendaAi.Domains.Entities;
using NUnit.Framework;
using System.Linq;

namespace MeAgendaAi.Unit.Domain
{
    public class UserTest
    {
        private readonly Faker Faker;

        public UserTest() => Faker = new Faker();

        [Test]
        public void ShouldCreateAUserWithAllValidFields()
        {
            var email = Faker.Internet.Email();
            var password = Faker.Internet.Password();

            var user = new User(email, password);

            user.Valid.Should().BeTrue();
            user.Invalid.Should().BeFalse();
        }

        [Test]
        public void ShouldCreateAUserWithAllValidFieldsAndNoValidationResult()
        {
            var email = Faker.Internet.Email();
            var password = Faker.Internet.Password();

            var user = new User(email, password);

            var errors = user.ValidationResult.Errors.ToList();
            errors.Should().BeEmpty();
        }

        [TestCase("", "E-mail cannot be empty")]
        [TestCase(null, "E-mail cannot be empty")]
        [TestCase("email", "Invalid e-mail")]
        [TestCase("email@", "Invalid e-mail")]
        public void ShouldCreateAnInvalidInstanceOfUserWithInvalidEmail(string email, string errorMessage)
        {
            var password = Faker.Internet.Password();

            var user = new User(email, password);

            var errors = user.ValidationResult.Errors;
            errors.Should().Contain(error => error.ErrorMessage == errorMessage);
            user.Valid.Should().BeFalse();
            user.Invalid.Should().BeTrue();
        }

        [TestCase("", "Password cannot be empty")]
        [TestCase(null, "Password cannot be empty")]
        public void ShouldCreateAnInvalidInstanceOfUserWithInvalidPassword(string password, string errorMessage)
        {
            var email = Faker.Internet.Email();

            var user = new User(email, password);

            var errors = user.ValidationResult.Errors;
            errors.Should().Contain(error => error.ErrorMessage == errorMessage);
            user.Valid.Should().BeFalse();
            user.Invalid.Should().BeTrue();
        }

        [TestCase(0, "Password cannot be empty")]
        [TestCase(2, "Password must contain at least 6 characters")]
        [TestCase(5, "Password must contain at least 6 characters")]
        [TestCase(33, "Password must contain a maximum of 32 characters")]
        [TestCase(40, "Password must contain a maximum of 32 characters")]
        public void ShouldCreateAnInvalidInstanceOfUserWithLengthInvalidPassword(int lengthPassword, string errorMessage)
        {
            var email = Faker.Internet.Email();
            string charsAccepted = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%*()/*-+";
            var password = Faker.Random.String2(lengthPassword, charsAccepted);

            var user = new User(email, password);

            var errors = user.ValidationResult.Errors;
            errors.Should().Contain(error => error.ErrorMessage == errorMessage);
            user.Valid.Should().BeFalse();
            user.Invalid.Should().BeTrue();
        }

    }
}
