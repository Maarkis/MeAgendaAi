using AutoBogus;
using Bogus;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Common.Builder.RequestAndResponse;

public sealed class AddPhysicalPersonRequestBuilder : AutoFaker<AddPhysicalPersonRequest>
{
	public AddPhysicalPersonRequestBuilder() : base("pt_BR")
	{
		var password = PasswordBuilder.Generate();
		RuleFor(prop => prop.Name, faker => faker.Name.FirstName());
		RuleFor(prop => prop.Email, faker => faker.Internet.Email());
		RuleFor(prop => prop.Password, () => password);
		RuleFor(prop => prop.ConfirmPassword, () => password);

		RuleFor(prop => prop.Surname, faker => faker.Name.LastName());
		RuleFor(prop => prop.CPF, faker => faker.Random.Int(11).ToString());
		RuleFor(prop => prop.RG, faker => faker.Random.Int(9).ToString());
		RuleFor(prop => prop.Phones, new PhoneRequestBuilder().Generate(1));
	}
}

public static class AddPhysicalPersonRequestBuilderExtensions
{
	public static AddPhysicalPersonRequestBuilder WithName(this AddPhysicalPersonRequestBuilder builder, string name)
	{
		builder.RuleFor(prop => prop.Name, () => name);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithNameInvalid(this AddPhysicalPersonRequestBuilder builder,
		string name = "")
	{
		builder.WithName(name);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithNameInvalid(this AddPhysicalPersonRequestBuilder builder,
		int length)
	{
		var name = new Faker().Random.String(length);
		builder.WithName(name);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithSurname(this AddPhysicalPersonRequestBuilder builder,
		string surname)
	{
		builder.RuleFor(prop => prop.Surname, () => surname);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithSurnameInvalid(this AddPhysicalPersonRequestBuilder builder,
		string surname = "")
	{
		builder.WithSurname(surname);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithSurnameInvalid(this AddPhysicalPersonRequestBuilder builder,
		int length)
	{
		var surname = new Faker().Random.String(length);
		builder.WithSurname(surname);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithEmail(this AddPhysicalPersonRequestBuilder builder, string email)
	{
		builder.RuleFor(prop => prop.Email, () => email);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithEmailInvalid(this AddPhysicalPersonRequestBuilder builder,
		string email = "")
	{
		builder.WithEmail(email);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithConfirmPassword(this AddPhysicalPersonRequestBuilder builder,
		string confirmPassword)
	{
		builder.RuleFor(prop => prop.ConfirmPassword, () => confirmPassword);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithConfirmPasswordInvalid(
		this AddPhysicalPersonRequestBuilder builder, string confirmPassword = "")
	{
		builder.WithConfirmPassword(confirmPassword);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithConfirmPasswordInvalid(
		this AddPhysicalPersonRequestBuilder builder, int length)
	{
		var confirmPassword = new Faker().Internet.Password(length);
		builder.WithConfirmPassword(confirmPassword);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithPassword(this AddPhysicalPersonRequestBuilder builder,
		string password)
	{
		builder.RuleFor(prop => prop.Password, () => password);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithPasswordInvalid(this AddPhysicalPersonRequestBuilder builder,
		string password = "")
	{
		builder.WithPassword(password);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithPasswordInvalid(this AddPhysicalPersonRequestBuilder builder,
		int length)
	{
		var password = new Faker().Internet.Password(length);
		builder.WithPassword(password);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithCpf(this AddPhysicalPersonRequestBuilder builder, string cpf)
	{
		builder.RuleFor(prop => prop.CPF, () => cpf);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithCpfInvalid(this AddPhysicalPersonRequestBuilder builder,
		string cpf = "")
	{
		builder.WithCpf(cpf);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithRg(this AddPhysicalPersonRequestBuilder builder, string rg)
	{
		builder.RuleFor(prop => prop.RG, () => rg);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithRgInvalid(this AddPhysicalPersonRequestBuilder builder,
		string rg = "")
	{
		builder.WithRg(rg);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithPhones(this AddPhysicalPersonRequestBuilder builder,
		IEnumerable<PhoneRequest>? phones)
	{
		builder.RuleFor(prop => prop.Phones, () => phones);
		return builder;
	}

	public static AddPhysicalPersonRequestBuilder WithPhonesInvalid(this AddPhysicalPersonRequestBuilder builder,
		IEnumerable<PhoneRequest>? phones = null)
	{
		builder.WithPhones(phones);
		return builder;
	}
}