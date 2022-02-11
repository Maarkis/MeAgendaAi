using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Common.Builder
{
    public class UserBuilder : BaseBuilderEntity<User>
    {
        public UserBuilder() : base()
        {
            RuleFor(prop => prop.Email, () => new EmailObjectBulder().Generate());
            RuleFor(prop => prop.Password, faker => faker.Internet.Password());
        }
        public override User Generate(string ruleSets = null!)
        {
            var valueObjets = base.Generate(ruleSets);
            valueObjets.Validate(valueObjets, new UserValidator<User>());
            return valueObjets;
        }
    }

    public static class UserBuilderBuilderExtensions
    {
        public static UserBuilder WithEmail(this UserBuilder builder, string email)
        {
            builder.RuleFor(prop => prop.Email, () => new EmailObjectBulder().WithEmail(email).Generate());
            return builder;
        }
        public static UserBuilder WithEmail(this UserBuilder builder, EmailObject email)
        {
            builder.RuleFor(prop => prop.Email, () => email);
            return builder;
        }
        public static UserBuilder WithPassword(this UserBuilder builder, string password)
        {
            builder.RuleFor(prop => prop.Password, () => password);
            return builder;
        }
    }
}
