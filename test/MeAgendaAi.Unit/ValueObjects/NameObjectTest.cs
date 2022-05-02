using Bogus;
using FluentAssertions;
using MeAgendaAi.Domains.ValueObjects;
using NUnit.Framework;

namespace MeAgendaAi.Unit.ValueObjects;

public class NameObjectTest
{
	private readonly Faker Faker;

	public NameObjectTest()
	{
		Faker = new Faker();
	}

	[Test]
	public void ShouldCreatedAnInstanceValidOfTypeNameObject()
	{
		var name = Faker.Name.FirstName();
		var surname = Faker.Name.LastName();

		var nameObject = new Name(name, surname);

		nameObject.Valid.Should().BeTrue();
		nameObject.ValidationResult.Errors.Should().BeEmpty();
	}

	[Test]
	public void ShouldCreatedAnInstanceValidOfTypeNameObjectWithCorrectValues()
	{
		var name = Faker.Name.FirstName();
		var surname = Faker.Name.LastName();

		var nameObject = new Name(name, surname);

		nameObject.FirstName.Should().Be(name);
		nameObject.Surname.Should().Be(surname);
	}

	[Test]
	public void ShouldConcatenateNameAndSurnameToFormFullName()
	{
		var name = Faker.Name.FirstName();
		var surname = Faker.Name.LastName();
		var fullNameExpected = $"{name} {surname}";

		var nameObject = new Name(name, surname);

		nameObject.FullName.Should().Be(fullNameExpected);
	}

	[Test]
	public void ShouldConcatenateNameToFormFullName()
	{
		var name = Faker.Name.FirstName();
		var fullNameExpected = $"{name}";

		var nameObject = new Name(name);

		nameObject.FullName.Should().Be(fullNameExpected);
	}

	[TestCase("Name cannot be empty", 0)]
	[TestCase("Name must contain at least 3 characters", 1)]
	[TestCase("Name must contain at least 3 characters", 2)]
	[TestCase("Name must contain a maximum of 60 characters", 61)]
	[TestCase("Name must contain a maximum of 60 characters", 100)]
	public void ShouldCreatedAnInvalidInstanceOfNameObjectWithErrorsInNameProperty(string messageError,
		int lengthSurname)
	{
		var name = Faker.Random.String(lengthSurname);

		var nameObject = new Name(name);

		nameObject.Valid.Should().BeFalse();
		var errors = nameObject.ValidationResult.Errors;
		errors.Should().Contain(error => error.ErrorMessage == messageError);
	}

	[TestCase("Surname cannot be empty", 0)]
	[TestCase("Surname must contain at least 3 characters", 1)]
	[TestCase("Surname must contain at least 3 characters", 2)]
	[TestCase("Surname must contain a maximum of 80 characters", 81)]
	[TestCase("Surname must contain a maximum of 80 characters", 100)]
	public void ShouldCreatedAnInvalidInstanceOfNameObjectWithErrorsInSurnameProperty(string messageError,
		int lengthSurname)
	{
		var name = Faker.Name.FirstName();
		var surname = Faker.Random.String2(lengthSurname);

		var nameObject = new Name(name, surname);

		nameObject.Valid.Should().BeFalse();
		var errors = nameObject.ValidationResult.Errors;
		errors.Should().Contain(error => error.ErrorMessage == messageError);
	}

	[Test]
	public void EqualsShouldBeTrueWhenTwoNameObjectWithSameName()
	{
		var firstName = Faker.Person.FirstName;
		var lastName = Faker.Person.LastName;
		var name = new Name(firstName, lastName);
		var otherName = new Name(firstName, lastName);

		name.Equals(otherName).Should().BeTrue();
	}

	[Test]
	public void EqualsShouldBeFalseTwoEmailObjectWithDifferentEmail()
	{
		var name = new Name(Faker.Person.FirstName, Faker.Person.LastName);
		var otherName = new Name("other name", "other lastName");

		name.Equals(otherName).Should().BeFalse();
	}

	[Test]
	public void EqualsShouldBeFalseWhenCheckObjectNameWithNullObjectName()
	{
		var name = new Name(Faker.Person.FirstName, Faker.Person.LastName);
		Name otherName = null!;

		name.Equals(otherName).Should().BeFalse();
	}

	[Test]
	public void EqualsShouldBeFalseWhenCheckObjectNameWithOtherObjectType()
	{
		var name = new Name(Faker.Person.FirstName, Faker.Person.LastName);
		var differentObject = new Email(Faker.Internet.Email());

		name.Equals(differentObject).Should().BeFalse();
	}
}