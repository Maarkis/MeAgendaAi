using AutoBogus;
using Bogus;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Common.Builder.RequestAndResponse;

public sealed class AddCompanyRequestBuilder : AutoFaker<AddCompanyRequest>
{
	public AddCompanyRequestBuilder() : base("pt_BR")
	{
		var password = new Faker().Internet.Password();
		RuleFor(prop => prop.Email, faker => faker.Internet.Email());
		RuleFor(prop => prop.Password, () => password);
		RuleFor(prop => prop.ConfirmPassword, () => password);

		RuleFor(prop => prop.Name, faker => faker.Company.CompanyName());
		RuleFor(prop => prop.CNPJ, faker => faker.Random.Int(14).ToString());
		RuleFor(prop => prop.Description, faker => faker.Lorem.Sentences(1));
		RuleFor(prop => prop.LimitCancelHours, faker => faker.Random.Int(1, 24));
		RuleFor(prop => prop.Phones, new PhoneRequestBuilder().Generate(1));
	}
}

public static class AddCompanyRequestBuilderExtensions
{
	public static AddCompanyRequestBuilder WithName(this AddCompanyRequestBuilder builder, string name)
	{
		builder.RuleFor(prop => prop.Name, () => name);
		return builder;
	}

	public static AddCompanyRequestBuilder WithNameInvalid(this AddCompanyRequestBuilder builder, string name = "")
	{
		builder.WithName(name);
		return builder;
	}

	public static AddCompanyRequestBuilder WithNameInvalid(this AddCompanyRequestBuilder builder, int length)
	{
		var name = new Faker().Random.String(length);
		builder.WithName(name);
		return builder;
	}

	public static AddCompanyRequestBuilder WithEmail(this AddCompanyRequestBuilder builder, string email)
	{
		builder.RuleFor(prop => prop.Email, () => email);
		return builder;
	}

	public static AddCompanyRequestBuilder WithEmailInvalid(this AddCompanyRequestBuilder builder, string email = "")
	{
		builder.WithEmail(email);
		return builder;
	}

	public static AddCompanyRequestBuilder WithPassword(this AddCompanyRequestBuilder builder, string password)
	{
		builder.RuleFor(prop => prop.Password, () => password);
		return builder;
	}

	public static AddCompanyRequestBuilder WithPasswordInvalid(this AddCompanyRequestBuilder builder,
		string password = "")
	{
		builder.WithPassword(password);
		return builder;
	}

	public static AddCompanyRequestBuilder WithPasswordInvalid(this AddCompanyRequestBuilder builder, int length)
	{
		var password = new Faker().Internet.Password(length);
		builder.WithPassword(password);
		return builder;
	}

	public static AddCompanyRequestBuilder WithConfirmPassword(this AddCompanyRequestBuilder builder,
		string confirmPassword)
	{
		builder.RuleFor(prop => prop.ConfirmPassword, () => confirmPassword);
		return builder;
	}

	public static AddCompanyRequestBuilder WithConfirmPasswordInvalid(this AddCompanyRequestBuilder builder,
		string confirmPassword = "")
	{
		builder.WithConfirmPassword(confirmPassword);
		return builder;
	}

	public static AddCompanyRequestBuilder WithConfirmPasswordInvalid(this AddCompanyRequestBuilder builder, int length)
	{
		var confirmPassword = new Faker().Internet.Password(length);
		builder.WithConfirmPassword(confirmPassword);
		return builder;
	}

	public static AddCompanyRequestBuilder WithCnpj(this AddCompanyRequestBuilder builder, string cnpj)
	{
		builder.RuleFor(prop => prop.CNPJ, () => cnpj);
		return builder;
	}

	public static AddCompanyRequestBuilder WithCnpjInvalid(this AddCompanyRequestBuilder builder, string cnpj = "")
	{
		builder.WithCnpj(cnpj);
		return builder;
	}

	public static AddCompanyRequestBuilder WithDescription(this AddCompanyRequestBuilder builder, string description)
	{
		builder.RuleFor(prop => prop.Description, () => description);
		return builder;
	}

	public static AddCompanyRequestBuilder WithDescriptionInvalid(this AddCompanyRequestBuilder builder,
		string description = "")
	{
		builder.WithDescription(description);
		return builder;
	}

	public static AddCompanyRequestBuilder WithDescriptionInvalid(this AddCompanyRequestBuilder builder, int length)
	{
		var description = new Faker().Random.String(length);
		builder.WithDescription(description);
		return builder;
	}

	public static AddCompanyRequestBuilder WithLimitCancelHours(this AddCompanyRequestBuilder builder,
		int limitCancelHours)
	{
		builder.RuleFor(prop => prop.LimitCancelHours, () => limitCancelHours);
		return builder;
	}

	public static AddCompanyRequestBuilder WithPhones(this AddCompanyRequestBuilder builder,
		IEnumerable<PhoneRequest>? phones)
	{
		builder.RuleFor(prop => prop.Phones, () => phones);
		return builder;
	}
	
	public static AddCompanyRequestBuilder WithPhonesInvalid(this AddCompanyRequestBuilder builder,
		IEnumerable<PhoneRequest>? phones = null)
	{
		builder.WithPhones(phones);
		return builder;
	}
}