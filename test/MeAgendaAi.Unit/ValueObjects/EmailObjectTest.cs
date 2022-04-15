using Bogus;
using FluentAssertions;
using MeAgendaAi.Domains.ValueObjects;
using NUnit.Framework;

namespace MeAgendaAi.Unit.ValueObjects
{
    public class EmailObjectTest
    {
        private readonly Faker Faker;

        public EmailObjectTest() => Faker = new Faker();

        [Test]
        public void ShouldCreatedAnInstanceValidOfTypeEmailObject()
        {
            var email = Faker.Internet.Email();

            var emailObject = new Email(email);

            emailObject.Valid.Should().BeTrue();
            emailObject.ValidationResult.Errors.Should().BeEmpty();
        }

        [Test]
        public void ShouldCreatedAnInstanceValidOfTypeEmailObjectWithCorrectValues()
        {
            var emailExpected = Faker.Internet.Email();

            var nameObject = new Email(emailExpected);

            nameObject.Address.Should().Be(emailExpected);
        }

        [TestCase("E-mail cannot be empty", null)]
        [TestCase("E-mail cannot be empty", "")]
        [TestCase("Invalid e-mail", "email@")]
        [TestCase("Invalid e-mail", "teste.email")]
        public void ShouldCreatedAnInvalidInstanceOfNameObjectWithErrorsInNameProperty(string messageError, string email)
        {
            var nameObject = new Email(email);

            nameObject.Valid.Should().BeFalse();
            var errors = nameObject.ValidationResult.Errors;
            errors.Should().Contain(error => error.ErrorMessage == messageError);
        }

        [Test]
        public void EqualsShouldBeTrueWhenTwoEmailObjectWithSameEmail()
        {
            var address = Faker.Internet.Email();
            var email = new Email(address);
            var otherEmail = new Email(address);

            email.Equals(otherEmail).Should().BeTrue();
        }

        [Test]
        public void EqualsShouldBeFalseTwoEmailObjectWithDifferentEmail()
        {
            var email = new Email(Faker.Internet.Email());
            var otherEmail = new Email(Faker.Internet.Email());

            email.Equals(otherEmail).Should().BeFalse();
        }

        [Test]
        public void EqualsShouldBeFalseWhenCheckObjectEmailWithNullObjectEmail()
        {
            var email = new Email(Faker.Internet.Email());
            Email otherEmail = null!;

            email.Equals(otherEmail).Should().BeFalse();
        }

        [Test]
        public void EqualsShouldBeFalseWhenCheckObjectEmailWithOtherObjectType()
        {
            var email = new Email(Faker.Internet.Email());
            var differentObject = new Name("", "");

            email.Equals(differentObject).Should().BeFalse();
        }
    }
}