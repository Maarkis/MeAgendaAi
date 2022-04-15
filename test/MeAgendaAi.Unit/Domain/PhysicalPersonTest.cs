using Bogus;
using FluentAssertions;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Entities;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Domain
{
    public class PhysicalPersonTest
    {
        private readonly Faker _faker;

        public PhysicalPersonTest() => _faker = new Faker();

        [Test]
        public void ShouldCreatedAnInstanceValidOfTypePhysicalPerson()
        {
            var email = _faker.Internet.Email();
            var password = PasswordBuilder.Generate();
            var name = _faker.Name.FirstName();
            var surname = _faker.Name.LastName();
            var cpf = _faker.Random.Int(11).ToString();
            var rg = _faker.Random.Int(9).ToString();

            var physicalPerson = new PhysicalPerson(email, password, name, surname, cpf, rg);

            physicalPerson.Valid.Should().BeTrue();
            physicalPerson.ValidationResult.Errors.Should().BeEmpty();
        }

        [Test]
        public void ShouldCreatedAnInstanceValidOfTypePhysicalPersonWithCorrectValues()
        {
            var physicalPersonExpected = new
            {
                Email = new EmailObjectBuilder().Generate(),
                Password = PasswordBuilder.Generate(),
                Name = new NameObjectBuilder().Generate(),
                CPF = _faker.Random.Int(11).ToString(),
                RG = _faker.Random.Int(9).ToString(),
            };

            var physicalPerson = new PhysicalPerson(
                physicalPersonExpected.Email.Address,
                physicalPersonExpected.Password,
                physicalPersonExpected.Name.FirstName,
                physicalPersonExpected.Name.Surname,
                physicalPersonExpected.CPF,
                physicalPersonExpected.RG);

            physicalPerson.Should().BeEquivalentTo(physicalPersonExpected, options => options.ExcludingMissingMembers());
        }
    }
}