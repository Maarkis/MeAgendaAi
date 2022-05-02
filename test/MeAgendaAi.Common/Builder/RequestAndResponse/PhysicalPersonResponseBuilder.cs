using AutoBogus;
using Bogus;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Common.Builder.RequestAndResponse;

public class PhysicalPersonResponseBuilder : AutoFaker<PhysicalPersonResponse>
{
	public PhysicalPersonResponseBuilder()
	{
		var faker = new Faker();
		var name = faker.Name.FirstName();
		var fullName = $"{name}";
		RuleFor(method => method.Id, () => Guid.NewGuid());
		RuleFor(method => method.Name, () => name);
		RuleFor(method => method.FullName, () => fullName);
		RuleFor(method => method.Email, () => faker.Internet.Email());
		RuleFor(method => method.CPF, () => faker.Random.Int(11).ToString());
	}
}

public static class PhysicalPersonResponseBuilderExtensions
{
	public static PhysicalPersonResponseBuilder WithId(this PhysicalPersonResponseBuilder builder, Guid id)
	{
		builder.RuleFor(method => method.Id, () => id);
		return builder;
	}

	public static PhysicalPersonResponseBuilder WithName(this PhysicalPersonResponseBuilder builder, string name)
	{
		builder.RuleFor(method => method.Name, () => name);
		return builder;
	}

	public static PhysicalPersonResponseBuilder WithFullName(this PhysicalPersonResponseBuilder builder,
		string fullName)
	{
		builder.RuleFor(method => method.FullName, () => fullName);
		return builder;
	}

	public static PhysicalPersonResponseBuilder WithEmail(this PhysicalPersonResponseBuilder builder, string email)
	{
		builder.RuleFor(method => method.Email, () => email);
		return builder;
	}

	public static PhysicalPersonResponseBuilder WithCPF(this PhysicalPersonResponseBuilder builder, string cpf)
	{
		builder.RuleFor(method => method.CPF, () => cpf);
		return builder;
	}

	public static PhysicalPersonResponseBuilder WithPhysicalPerson(this PhysicalPersonResponseBuilder builder,
		PhysicalPerson physicalPerson)
	{
		builder
			.WithId(physicalPerson.Id)
			.WithName(physicalPerson.Name.FirstName)
			.WithFullName(physicalPerson.Name.FullName)
			.WithEmail(physicalPerson.Email.Address)
			.WithCPF(physicalPerson.CPF);
		return builder;
	}
}