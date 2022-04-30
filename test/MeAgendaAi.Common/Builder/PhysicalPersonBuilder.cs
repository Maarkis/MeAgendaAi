using Bogus;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Common.Builder
{
    public sealed class PhysicalPersonBuilder : BaseBuilderEntity<PhysicalPerson>
    {
        public PhysicalPersonBuilder() : base()
        {
            RuleFor(x => x.Email, () => new EmailObjectBuilder().Generate());
            RuleFor(x => x.Password, PasswordBuilder.Generate());
            RuleFor(x => x.Name, () => new NameObjectBuilder().Generate());

            RuleFor(x => x.Cpf, faker => faker.Random.Int(11).ToString());
            RuleFor(x => x.Rg, faker => faker.Random.Int(9).ToString());
            RuleFor(prop => prop.IsActive, () => false);
        }

        public override PhysicalPerson Generate(string ruleSets = null!)
        {
            var entity = base.Generate(ruleSets);
            entity.Validate();
            return entity;
        }
    }

    public static class PhysicalPersonBuilderExtensions
    {
        public static PhysicalPersonBuilder WithId(this PhysicalPersonBuilder builder, Guid id)
        {
            builder.RuleFor(x => x.Id, () => id);
            return builder;
        }

        public static PhysicalPersonBuilder WithEmail(this PhysicalPersonBuilder builder, string email)
        {
            builder.RuleFor(x => x.Email, () => new EmailObjectBuilder().WithEmail(email).Generate());
            return builder;
        }

        public static PhysicalPersonBuilder WithEmail(this PhysicalPersonBuilder builder, Email email)
        {
            builder.RuleFor(x => x.Email, () => email);
            return builder;
        }

        public static PhysicalPersonBuilder WithPassword(this PhysicalPersonBuilder builder, string password)
        {
            builder.RuleFor(x => x.Password, () => password);
            return builder;
        }

        public static PhysicalPersonBuilder WithName(this PhysicalPersonBuilder builder, string name)
        {
            builder.RuleFor(x => x.Name, () => new NameObjectBuilder().WithName(name).Generate());
            return builder;
        }

        public static PhysicalPersonBuilder WithSurname(this PhysicalPersonBuilder builder, string surname)
        {
            builder.RuleFor(x => x.Name, () => new NameObjectBuilder().WithSurname(surname).Generate());
            return builder;
        }

        public static PhysicalPersonBuilder WithFullName(this PhysicalPersonBuilder builder, Name name)
        {
            builder.RuleFor(x => x.Name, () => name);
            return builder;
        }

        public static PhysicalPersonBuilder WithNameAndSurname(this PhysicalPersonBuilder builder, string name, string surname)
        {
            var fullName = new NameObjectBuilder().WithName(name).WithSurname(surname).Generate();
            builder.WithFullName(fullName);
            return builder;
        }

        public static PhysicalPersonBuilder WithCPF(this PhysicalPersonBuilder builder, string cpf)
        {
            builder.RuleFor(x => x.Cpf, () => cpf);
            return builder;
        }

        public static PhysicalPersonBuilder WithRG(this PhysicalPersonBuilder builder, string rg)
        {
            builder.RuleFor(x => x.Rg, () => rg);
            return builder;
        }

        public static PhysicalPersonBuilder WithNameInvalid(this PhysicalPersonBuilder builder, string name = "")
        {
            builder.WithName(name);
            return builder;
        }

        public static PhysicalPersonBuilder WithNameInvalidByLength(this PhysicalPersonBuilder builder, int length = 0)
        {
            var name = new Faker().Random.String(length);
            builder.WithName(name);
            return builder;
        }

        public static PhysicalPersonBuilder WithSurnameInvalidByLength(this PhysicalPersonBuilder builder, int length = 0)
        {
            var surname = new Faker().Random.String(length);
            builder.WithSurname(surname);
            return builder;
        }

        public static PhysicalPersonBuilder WithEmailInvalid(this PhysicalPersonBuilder builder, string email = "")
        {
            builder.WithEmail(email);
            return builder;
        }

        public static PhysicalPersonBuilder WithPasswordInvalid(this PhysicalPersonBuilder builder, string password = "")
        {
            builder.WithPassword(password);
            return builder;
        }

        public static PhysicalPersonBuilder WithPasswordInvalidByLength(this PhysicalPersonBuilder builder, int length = 0)
        {
            var password = new Faker().Random.String(length);
            builder.WithPassword(password);
            return builder;
        }

        public static PhysicalPersonBuilder WithCPFInvalid(this PhysicalPersonBuilder builder, string cpf = "")
        {
            builder.WithCPF(cpf);
            return builder;
        }

        public static PhysicalPersonBuilder WithRGInvalid(this PhysicalPersonBuilder builder, string rg = "")
        {
            builder.WithRG(rg);
            return builder;
        }

        public static PhysicalPersonBuilder WithActive(this PhysicalPersonBuilder builder, bool active)
        {
            builder.RuleFor(prop => prop.IsActive, () => active);
            return builder;
        }

        public static PhysicalPersonBuilder ByRequest(this PhysicalPersonBuilder builder, AddPhysicalPersonRequest request)
        {
            builder.WithNameAndSurname(request.Name, request.Surname)
                .WithEmail(request.Email)
                .WithPassword(request.Password)
                .WithCPF(request.CPF)
                .WithRG(request.RG);
            return builder;
        }
    }
}