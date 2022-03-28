using Bogus;
using FluentAssertions;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Entities;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Domain
{
    public class CompanyTest
    {
        private readonly Faker _faker;

        public CompanyTest() => _faker = new Faker("pt_BR");

        [Test]
        public void ShouldCreatedAnInstanceValidOfTypeCompany()
        {
            var email = _faker.Internet.Email();
            var password = PasswordBuilder.Generate();
            var name = _faker.Name.FirstName();
            var cnpj = _faker.Random.Int(15).ToString();
            var description = _faker.Lorem.Word();
            var limitCancelHours = _faker.Random.Int();

            var company = new Company(email, password, name, cnpj, description, limitCancelHours);

            company.Valid.Should().BeTrue();
            company.ValidationResult.Errors.Should().BeEmpty();
        }

        [Test]
        public void ShouldCreatedAnInstanceValidOfTypeCompanyWithCorrectValues()
        {
            var companyExpected = new
            {
                Email = new EmailObjectBuilder().Generate(),
                Password = PasswordBuilder.Generate(),
                Name = new NameObjectBuilder().WithoutSurname().Generate(validateSurname: false),
                CNPJ = _faker.Random.Int(15).ToString(),
                Description = _faker.Lorem.Paragraph(min: 1),
                LimitCancelHours = _faker.Random.Int()
            };

            var company = new Company(
                companyExpected.Email.Email,
                companyExpected.Password,
                companyExpected.Name.Name,
                companyExpected.CNPJ,
                companyExpected.Description,
                companyExpected.LimitCancelHours);

            company.Should().BeEquivalentTo(companyExpected, options => options.ExcludingMissingMembers());
        }
    }
}