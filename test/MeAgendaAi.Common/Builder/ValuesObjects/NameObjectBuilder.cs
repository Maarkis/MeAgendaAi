using MeAgendaAi.Domains.Validators.ValueObjects;
using MeAgendaAi.Domains.ValueObjects;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MeAgendaAi.Common.Builder.ValuesObjects;

public sealed class NameObjectBuilder : BaseBuilderValueObject<Name>
{
	private bool _validateSurname = true;
	public NameObjectBuilder()
	{
		RuleFor(prop => prop.FirstName, faker => faker.Name.FirstName());
		RuleFor(prop => prop.Surname, faker => faker.Name.LastName());
	}

	public Name Generate(bool validateSurname = false, string ruleSets = null!)
	{
		var valueObjets = base.Generate(ruleSets);
		valueObjets.Validate(valueObjets,
			validateSurname ? 
				new NameValidator() : 
				new NameValidator(includeValidateSurname: _validateSurname));
		return valueObjets;
	}

	public override Name Generate(string ruleSets = null!)
	{
		var valueObjets = base.Generate(ruleSets);
		valueObjets.Validate(valueObjets, new NameValidator());
		return valueObjets;
	}

	public NameObjectBuilder NotValidateName()
	{
		_validateSurname = false;
		return this;
	}
}

public static class NameObjectBuilderExtensions
{
	public static NameObjectBuilder WithName(this NameObjectBuilder builder, string name)
	{
		builder.RuleFor(prop => prop.FirstName, () => name);
		return builder;
	}

	public static NameObjectBuilder WithSurname(this NameObjectBuilder builder)
	{
		builder.RuleFor(prop => prop.Surname, faker => faker.Name.LastName());
		return builder;
	}

	public static NameObjectBuilder WithSurname(this NameObjectBuilder builder, string surname)
	{
		builder.RuleFor(prop => prop.Surname, () => surname);
		return builder;
	}

	public static NameObjectBuilder WithoutSurname(this NameObjectBuilder builder)
	{
		builder.NotValidateName().RuleFor(prop => prop.Surname, string.Empty);
		return builder;
	}
}