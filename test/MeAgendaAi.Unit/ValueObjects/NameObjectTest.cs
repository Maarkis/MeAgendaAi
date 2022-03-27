using Bogus;
using FluentAssertions;
using MeAgendaAi.Domains.ValueObjects;
using NUnit.Framework;

namespace MeAgendaAi.Unit.ValueObjects
{
    public class NameObjectTest
    {
        private readonly Faker Faker;

        public NameObjectTest() => Faker = new Faker();

        [Test]
        public void ShouldCreatedAnInstanceValidOfTypeNameObject()
        {
            var name = Faker.Name.FirstName();
            var surname = Faker.Name.LastName();

            var nameObject = new NameObject(name, surname);

            nameObject.Valid.Should().BeTrue();
            nameObject.ValidationResult.Errors.Should().BeEmpty();
        }

        [Test]
        public void ShouldCreatedAnInstanceValidOfTypeNameObjectWithCorrectValues()
        {
            var name = Faker.Name.FirstName();
            var surname = Faker.Name.LastName();

            var nameObject = new NameObject(name, surname);

            nameObject.Name.Should().Be(name);
            nameObject.Surname.Should().Be(surname);
        }

        [Test]
        public void ShouldConcatenateNameAndSurnameToFormFullName()
        {
            var name = Faker.Name.FirstName();
            var surname = Faker.Name.LastName();
            var fullNameExpected = $"{name} {surname}";

            var nameObject = new NameObject(name, surname);

            nameObject.FullName.Should().Be(fullNameExpected);
        }

        [Test]
        public void ShouldConcatenateNameToFormFullName()
        {
            var name = Faker.Name.FirstName();
            var fullNameExpected = $"{name}";

            var nameObject = new NameObject(name);

            nameObject.FullName.Should().Be(fullNameExpected);
        }

        [TestCase("Name cannot be empty", 0)]
        [TestCase("Name must contain at least 3 characters", 1)]
        [TestCase("Name must contain at least 3 characters", 2)]
        [TestCase("Name must contain a maximum of 60 characters", 61)]
        [TestCase("Name must contain a maximum of 60 characters", 100)]
        public void ShouldCreatedAnInvalidInstanceOfNameObjectWithErrorsInNameProperty(string messageError, int lengthSurname)
        {
            var name = Faker.Random.String(lengthSurname);
            var surname = Faker.Name.LastName();
            var fullNameExpected = $"{name} {surname}";

            var nameObject = new NameObject(name);

            nameObject.Valid.Should().BeFalse();
            var errors = nameObject.ValidationResult.Errors;
            errors.Should().Contain(error => error.ErrorMessage == messageError);
        }

        [TestCase("Surname cannot be empty", 0)]
        [TestCase("Surname must contain at least 3 characters", 1)]
        [TestCase("Surname must contain at least 3 characters", 2)]
        [TestCase("Surname must contain a maximum of 80 characters", 81)]
        [TestCase("Surname must contain a maximum of 80 characters", 100)]
        public void ShouldCreatedAnInvalidInstanceOfNameObjectWithErrorsInSurnameProperty(string messageError, int lengthSurname)
        {
            var name = Faker.Name.FirstName();
            var surname = Faker.Random.String2(lengthSurname);
            var fullNameExpected = $"{name} {surname}";

            var nameObject = new NameObject(name, surname);

            nameObject.Valid.Should().BeFalse();
            var errors = nameObject.ValidationResult.Errors;
            errors.Should().Contain(error => error.ErrorMessage == messageError);
        }
    }
}