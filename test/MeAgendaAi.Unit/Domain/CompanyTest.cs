using System.Collections.Generic;
using Bogus;
using FluentAssertions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Entities;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Domain;

public class CompanyTest
{
	private readonly Faker _faker;
	private readonly PhoneNumberBuilder _phoneNumberBuilder;

	public CompanyTest()
	{
		_faker = new Faker("pt_BR");
		_phoneNumberBuilder = new PhoneNumberBuilder();
	}

	[Test]
	public void ShouldCreatedAnInstanceValidOfTypeCompany()
	{
		var email = _faker.Internet.Email();
		var password = PasswordBuilder.Generate();
		var name = _faker.Name.FirstName();
		var cnpj = _faker.Random.Int(15).ToString();
		var description = _faker.Random.String2(_faker.Random.Int(1, 160));
		var limitCancelHours = _faker.Random.Int(1);
		var phones = _phoneNumberBuilder.Generate(1);
		
		var company = new Company(email, password, name, cnpj, description, limitCancelHours, phones);

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
			Name = new NameObjectBuilder().WithoutSurname().Generate(false),
			CNPJ = _faker.Random.Int(15).ToString(),
			Description = _faker.Random.String2(_faker.Random.Int(1, 160)),
			LimitCancelHours = _faker.Random.Int(),
			Phones = _phoneNumberBuilder.Generate(1)
		};

		var company = new Company(
			companyExpected.Email.Address,
			companyExpected.Password,
			companyExpected.Name.FirstName,
			companyExpected.CNPJ,
			companyExpected.Description,
			companyExpected.LimitCancelHours,
			companyExpected.Phones);

		company.Should().BeEquivalentTo(companyExpected, options => options.ExcludingMissingMembers());
	}
}