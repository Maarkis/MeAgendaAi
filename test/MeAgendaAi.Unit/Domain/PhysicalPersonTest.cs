using Bogus;
using FluentAssertions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Validators;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Domain
{
    public class PhysicalPersonTest
    {
        private readonly Faker Faker;

        public PhysicalPersonTest() => Faker = new Faker();

        [Test]
        public void ShouldCreatedAnInstanceValidOfTypePhysicalPerson()
        {
            var email = Faker.Internet.Email();
            var password = Faker.Internet.Password();
            var name = Faker.Name.FirstName();
            var surname = Faker.Name.LastName();
            var cpf = Faker.Random.Int(11).ToString();
            var rg = Faker.Random.Int(9).ToString();

            var physicalPerson = new PhysicalPerson(email, password, name, surname, cpf, rg);

            physicalPerson.Valid.Should().BeTrue();
            physicalPerson.ValidationResult.Errors.Should().BeEmpty();
        }

        [Test]
        public void ShouldCreatedAnInstanceValidOfTypePhysicalPersonWithCorrectValues()
        {
            var physicalPersonExpected = new PhysicalPersonBuilder().Generate();

            var physicalPerson = new PhysicalPerson(physicalPersonExpected.Email.Email, physicalPersonExpected.Password,
                                                    physicalPersonExpected.Name.Name, physicalPersonExpected.Name.Surname,
                                                    physicalPersonExpected.CPF, physicalPersonExpected.RG);

            physicalPerson.Should().BeEquivalentTo(physicalPersonExpected,
                options => options.Excluding(prop => prop.Id)
                                  .Excluding(prop => prop.CreatedAt)
                                  .Excluding(prop => prop.LastUpdatedAt));
        }
    }
}
