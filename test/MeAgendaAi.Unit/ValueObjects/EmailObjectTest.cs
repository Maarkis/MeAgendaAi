using Bogus;
using FluentAssertions;
using MeAgendaAi.Domains.ValueObjects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var emailObject = new EmailObject(email);

            emailObject.Valid.Should().BeTrue();
            emailObject.ValidationResult.Errors.Should().BeEmpty();
        }

        [Test]
        public void ShouldCreatedAnInstanceValidOfTypeEmailObjectWithCorrectValues()
        {
            var emailExpected = Faker.Internet.Email();

            var nameObject = new EmailObject(emailExpected);

            nameObject.Email.Should().Be(emailExpected);
        }

        [TestCase("E-mail cannot be empty", null)]
        [TestCase("E-mail cannot be empty", "")]        
        [TestCase("Invalid e-mail", "email@")]
        [TestCase("Invalid e-mail", "teste.email")]
        public void ShouldCreatedAnInvalidInstanceOfNameObjectWithErrorsInNameProperty(string messageError, string email)
        {
            var nameObject = new EmailObject(email);

            nameObject.Valid.Should().BeFalse();
            var errors = nameObject.ValidationResult.Errors;
            errors.Should().Contain(error => error.ErrorMessage == messageError);
        }
    }
}
