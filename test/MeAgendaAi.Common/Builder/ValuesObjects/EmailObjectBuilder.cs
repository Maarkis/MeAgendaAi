using MeAgendaAi.Domains.Validators.ValueObjects;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Common.Builder.ValuesObjects;

public class EmailObjectBuilder : BaseBuilderValueObject<Email>
{
	public EmailObjectBuilder()
	{
		RuleFor(prop => prop.Address, faker => faker.Internet.Email());
		RuleFor(prop => prop.Valid, () => true);
	}

	public override Email Generate(string ruleSets = null!)
	{
		var valueObjets = base.Generate(ruleSets);
		valueObjets.Validate(valueObjets, new EmailValidator());
		return valueObjets;
	}
}

public static class EmailObjectBulderExtensions
{
	public static EmailObjectBuilder WithEmail(this EmailObjectBuilder builder, string email)
	{
		builder.RuleFor(prop => prop.Address, () => email);
		return builder;
	}
}