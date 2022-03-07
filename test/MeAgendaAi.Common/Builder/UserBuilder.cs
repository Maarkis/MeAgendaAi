using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Common.Builder
{
    public class UserBuilder : BaseBuilderEntity<User>
    {
        public UserBuilder() : base()
        {
            RuleFor(prop => prop.Email, () => new EmailObjectBuilder().Generate());
            RuleFor(prop => prop.Password, faker => faker.Internet.Password());
            RuleFor(prop => prop.Name, () => new NameObjectBuilder().Generate());
        }

        public override User Generate(string ruleSets = null!)
        {
            var entity = base.Generate(ruleSets);
            entity.Validate(includeSurname: true);
            return entity;
        }
    }

    public static class UserBuilderBuilderExtensions
    {
        public static UserBuilder WithId(this UserBuilder builder, Guid id)
        {
            builder.RuleFor(prop => prop.Id, () => id);
            return builder;
        }

        public static UserBuilder WithEmail(this UserBuilder builder, string email)
        {
            builder.RuleFor(prop => prop.Email, () => new EmailObjectBuilder().WithEmail(email).Generate());
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

        public static UserBuilder WithName(this UserBuilder builder, string name)
        {
            builder.RuleFor(x => x.Name, () => new NameObjectBuilder().WithName(name).Generate());
            return builder;
        }
        public static UserBuilder WithName(this UserBuilder builder, NameObject name)
        {
            builder.RuleFor(x => x.Name, () => name);
            return builder;
        }
        public static UserBuilder WithNameAndSurname(this UserBuilder builder, string name, string surname)
        {
            builder.RuleFor(x => x.Name, () => new NameObjectBuilder().WithName(name).WithSurname(surname).Generate());
            return builder;
        }
    }
}