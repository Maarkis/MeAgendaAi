using System.Linq;
using Bogus;
using FluentAssertions;
using MeAgendaAi.Domains.Entities;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Domain;

public class UserTest
{
	private readonly Faker _faker;

	private const string CharsAccepted = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%*()/*-+";

	public UserTest() => _faker = new Faker();

	[Test]
	public void ShouldCreateAUserWithAllValidFields()
	{
		var email = _faker.Internet.Email();
		var password = _faker.Internet.Password();
		var name = _faker.Name.FirstName();

		var user = new User(email, password, name);

		user.Valid.Should().BeTrue();
		user.Invalid.Should().BeFalse();
	}

	[Test]
	public void ShouldCreateAUserWithAllValidFieldsAndNoValidationResult()
	{
		var email = _faker.Internet.Email();
		var password = _faker.Internet.Password();
		var name = _faker.Name.FirstName();

		var user = new User(email, password, name);

		var errors = user.ValidationResult.Errors.ToList();
		errors.Should().BeEmpty();
	}

	[TestCase("", "E-mail cannot be empty")]
	[TestCase(null, "E-mail cannot be empty")]
	[TestCase("email", "Invalid e-mail")]
	[TestCase("email@", "Invalid e-mail")]
	public void ShouldCreateAnInvalidInstanceOfUserWithInvalidEmail(string email, string errorMessage)
	{
		var name = _faker.Name.FirstName();
		var password = _faker.Internet.Password();

		var user = new User(email, password, name);

		var errors = user.ValidationResult.Errors;
		errors.Should().Contain(error => error.ErrorMessage == errorMessage);
		user.Valid.Should().BeFalse();
		user.Invalid.Should().BeTrue();
	}

	[TestCase("", "Password cannot be empty")]
	[TestCase(null, "Password cannot be empty")]
	public void ShouldCreateAnInvalidInstanceOfUserWithInvalidPassword(string password, string errorMessage)
	{
		var email = _faker.Internet.Email();
		var name = _faker.Name.FirstName();

		var user = new User(email, password, name);

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
		var email = _faker.Internet.Email();
		var name = _faker.Name.FirstName();
		var password = _faker.Random.String2(lengthPassword, CharsAccepted);

		var user = new User(email, password, name);

		var errors = user.ValidationResult.Errors;
		errors.Should().Contain(error => error.ErrorMessage == errorMessage);
		user.Valid.Should().BeFalse();
		user.Invalid.Should().BeTrue();
	}

	[TestCase("", "Name cannot be empty")]
	[TestCase(null, "Name cannot be empty")]
	public void ShouldCreateAnInvalidInstanceOfUserWithInvalidName(string name, string errorMessage)
	{
		var email = _faker.Internet.Email();
		var password = _faker.Internet.Password();

		var user = new User(email, password, name);

		var errors = user.ValidationResult.Errors;
		errors.Should().Contain(error => error.ErrorMessage == errorMessage);
		user.Valid.Should().BeFalse();
		user.Invalid.Should().BeTrue();
	}

	[TestCase(0, "Name cannot be empty")]
	[TestCase(1, "Name must contain at least 3 characters")]
	[TestCase(2, "Name must contain at least 3 characters")]
	[TestCase(61, "Name must contain a maximum of 60 characters")]
	[TestCase(100, "Name must contain a maximum of 60 characters")]
	[TestCase(101, "Name must contain a maximum of 60 characters")]
	public void ShouldCreateAnInvalidInstanceOfUserWithLengthInvalidName(int lengthName, string errorMessage)
	{
		var email = _faker.Internet.Email();
		var password = _faker.Internet.Password();
		var name = _faker.Random.String2(lengthName, CharsAccepted);

		var user = new User(email, password, name);

		var errors = user.ValidationResult.Errors;
		errors.Should().Contain(error => error.ErrorMessage == errorMessage);
		user.Valid.Should().BeFalse();
		user.Invalid.Should().BeTrue();
	}

	[TestCase("", "Surname cannot be empty")]
	[TestCase(null, "Surname cannot be empty")]
	public void ShouldCreateAnInvalidInstanceOfUserWithInvalidSurname(string surname, string errorMessage)
	{
		var email = _faker.Internet.Email();
		var password = _faker.Internet.Password();
		var name = _faker.Name.FirstName();

		var user = new User(email, password, name, surname);

		var errors = user.ValidationResult.Errors;
		errors.Should().Contain(error => error.ErrorMessage == errorMessage);
		user.Valid.Should().BeFalse();
		user.Invalid.Should().BeTrue();
	}

	[TestCase(0, "Surname cannot be empty")]
	[TestCase(1, "Surname must contain at least 3 characters")]
	[TestCase(2, "Surname must contain at least 3 characters")]
	[TestCase(81, "Surname must contain a maximum of 80 characters")]
	[TestCase(100, "Surname must contain a maximum of 80 characters")]
	[TestCase(101, "Surname must contain a maximum of 80 characters")]
	public void ShouldCreateAnInvalidInstanceOfUserWithLengthInvalidSurname(int lengthName, string errorMessage)
	{
		var email = _faker.Internet.Email();
		var password = _faker.Internet.Password();
		var name = _faker.Name.FirstName();
		var surname = _faker.Random.String2(lengthName, CharsAccepted);

		var user = new User(email, password, name, surname);

		var errors = user.ValidationResult.Errors;
		errors.Should().Contain(error => error.ErrorMessage == errorMessage);
		user.Valid.Should().BeFalse();
		user.Invalid.Should().BeTrue();
	}
}