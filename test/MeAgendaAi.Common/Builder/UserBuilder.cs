using Bogus;
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

        public static UserBuilder WithEmail(this UserBuilder builder, Email email)
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

        public static UserBuilder WithFullName(this UserBuilder builder, Name name)
        {
            builder.RuleFor(x => x.Name, () => name);
            return builder;
        }

        public static UserBuilder WithNameAndSurname(this UserBuilder builder, string name, string surname)
        {
            var fullName = new NameObjectBuilder().WithName(name).WithSurname(surname).Generate();
            builder.WithFullName(fullName);
            return builder;
        }

        public static UserBuilder WithNameInvalidByLength(this UserBuilder builder, int length = 0)
        {
            var name = new Faker().Random.String(length);
            var fullName = new NameObjectBuilder().WithName(name).Generate();
            builder.WithFullName(fullName);
            return builder;
        }

        public static UserBuilder WithSurnameInvalidByLength(this UserBuilder builder, int length = 0)
        {
            var surname = new Faker().Random.String(length);
            var fullName = new NameObjectBuilder().WithSurname(surname).Generate();
            builder.WithFullName(fullName);
            return builder;
        }

        public static UserBuilder WithNameAndSurnameInvalid(this UserBuilder builder, string name = "", string surname = "")
        {
            builder.WithNameAndSurname(name, surname);
            return builder;
        }

        public static UserBuilder WithNameAndSurnameInvalidByLength(this UserBuilder builder, int length = 0)
        {
            var name = new Faker().Random.String(length);
            var surname = new Faker().Random.String(length);
            var fullName = new NameObjectBuilder().WithName(name).WithSurname(surname).Generate();
            builder.WithFullName(fullName);
            return builder;
        }

        public static UserBuilder WithEmailInvalid(this UserBuilder builder, string email = "")
        {
            builder.WithEmail(email);
            return builder;
        }

        public static UserBuilder WithPasswordInvalid(this UserBuilder builder, string password = "")
        {
            builder.WithPassword(password);
            return builder;
        }

        public static UserBuilder WithPasswordInvalidByLength(this UserBuilder builder, int length = 0)
        {
            var password = new Faker().Random.String(length);
            builder.WithPassword(password);
            return builder;
        }
    }
}