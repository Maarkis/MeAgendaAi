using MeAgendaAi.Domains.Validators.ValueObjects;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Common.Builder.ValuesObjects
{
    public class EmailObjectBuilder : BaseBuilderValueObject<EmailObject>
    {
        public EmailObjectBuilder()
        {
            RuleFor(prop => prop.Email, faker => faker.Internet.Email());
            RuleFor(prop => prop.Valid, () => true);
        }

        public override EmailObject Generate(string ruleSets = null!)
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
            builder.RuleFor(prop => prop.Email, () => email);
            return builder;
        }
    }
}

