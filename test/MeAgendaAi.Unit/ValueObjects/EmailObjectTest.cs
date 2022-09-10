using Bogus;
using FluentAssertions;
using MeAgendaAi.Domains.ValueObjects;
using NUnit.Framework;

namespace MeAgendaAi.Unit.ValueObjects;

public class EmailObjectTest
{
	private readonly Faker _faker;

	public EmailObjectTest()
	{
		_faker = new Faker();
	}

	[Test]
	public void ShouldCreatedAnInstanceValidOfTypeEmailObject()
	{
		var email = _faker.Internet.Email();

		var emailObject = new Email(email);

		emailObject.Valid.Should().BeTrue();
		emailObject.ValidationResult.Errors.Should().BeEmpty();
	}

	[Test]
	public void ShouldCreatedAnInstanceValidOfTypeEmailObjectWithCorrectValues()
	{
		var emailExpected = _faker.Internet.Email();

		var emailObject = new Email(emailExpected);

		emailObject.Address.Should().Be(emailExpected);
	}

	[TestCase("E-mail cannot be empty", null)]
	[TestCase("E-mail cannot be empty", "")]
	[TestCase("Invalid e-mail", "email@")]
	[TestCase("Invalid e-mail", "teste.email")]
	public void ShouldCreatedAnInvalidInstanceOfNameObjectWithErrorsInNameProperty(string messageError, string email)
	{
		var emailObject = new Email(email);

		emailObject.Valid.Should().BeFalse();
		var errors = emailObject.ValidationResult.Errors;
		errors.Should().Contain(error => error.ErrorMessage == messageError);
	}

	[Test]
	public void ShouldReturnEmailDomainCorrectly()
	{
		var domain = _faker.Internet.DomainName();
		var email = _faker.Internet.Email(_faker.Name.FirstName(), _faker.Name.LastName(), domain);
		
		var emailObject = new Email(email);

		emailObject.Domain.Should().Be(domain);
	}
}