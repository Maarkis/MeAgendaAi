using Bogus;
using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Common.Builder
{
    public class CompanyBuilder : BaseBuilderEntity<Company>
    {
        public CompanyBuilder()
        {
            RuleFor(prop => prop.Email, () => new EmailObjectBuilder().Generate());
            RuleFor(prop => prop.Password, faker => faker.Internet.Password());

            RuleFor(prop => prop.Name, () => new NameObjectBuilder().WithoutSurname().Generate());
            RuleFor(prop => prop.CNPJ, faker => faker.Random.Int(14).ToString());
            RuleFor(prop => prop.Description, faker => faker.Lorem.Sentences(sentenceCount: 1));
            RuleFor(prop => prop.LimitCancelHours, faker => faker.Random.Int(min: 1, max: 24));
        }

        public override Company Generate(string ruleSets = null!)
        {
            var entity = base.Generate(ruleSets);
            entity.Validate();
            return entity;
        }
    }

    public static class CompanyBuilderExtensions
    {
        public static CompanyBuilder WithId(this CompanyBuilder builder, Guid id)
        {
            builder.RuleFor(prop => prop.Id, () => id);
            return builder;
        }

        public static CompanyBuilder WithName(this CompanyBuilder builder, string name)
        {
            builder.RuleFor(prop => prop.Name, () => new NameObjectBuilder().WithName(name).Generate());
            return builder;
        }

        public static CompanyBuilder WithName(this CompanyBuilder builder, NameObject name)
        {
            builder.RuleFor(x => x.Name, () => name);
            return builder;
        }

        public static CompanyBuilder WithEmail(this CompanyBuilder builder, string email)
        {
            builder.RuleFor(prop => prop.Email, () => new EmailObjectBuilder().WithEmail(email).Generate());
            return builder;
        }

        public static CompanyBuilder WithEmail(this CompanyBuilder builder, EmailObject email)
        {
            builder.RuleFor(x => x.Email, () => email);
            return builder;
        }

        public static CompanyBuilder WithPassword(this CompanyBuilder builder, string password)
        {
            builder.RuleFor(x => x.Password, () => password);
            return builder;
        }

        public static CompanyBuilder WithCNPJ(this CompanyBuilder builder, string cnpj)
        {
            builder.RuleFor(prop => prop.CNPJ, () => cnpj);
            return builder;
        }
        public static CompanyBuilder WithDescription(this CompanyBuilder builder, string description)
        {
            builder.RuleFor(prop => prop.Description, () => description);
            return builder;
        }

        public static CompanyBuilder WithLimitCancelHours(this CompanyBuilder builder, int limitCancelHours)
        {
            builder.RuleFor(prop => prop.LimitCancelHours, () => limitCancelHours);
            return builder;
        }

        public static CompanyBuilder WithNameInvalid(this CompanyBuilder builder, string name = "")
        {
            builder.WithName(name);
            return builder;
        }

        public static CompanyBuilder WithNameInvalidByLength(this CompanyBuilder builder, int length = 0)
        {
            var name = new Faker().Random.String(length);
            builder.WithName(name);
            return builder;
        }

        public static CompanyBuilder WithEmailInvalid(this CompanyBuilder builder, string email = "")
        {
            builder.WithEmail(email);
            return builder;
        }

        public static CompanyBuilder WithPasswordInvalid(this CompanyBuilder builder, string password = "")
        {
            builder.RuleFor(x => x.Password, () => password);
            return builder;
        }

        public static CompanyBuilder WithPasswordInvalidByLength(this CompanyBuilder builder, int length = 0)
        {
            var password = new Faker().Random.String(length);
            builder.WithPassword(password);
            return builder;
        }

        public static CompanyBuilder WithCNPJInvalid(this CompanyBuilder builder, string cnpj = "")
        {
            builder.RuleFor(prop => prop.CNPJ, () => cnpj);
            return builder;
        }

        public static CompanyBuilder WithDescriptionInvalid(this CompanyBuilder builder, string description = "")
        {
            builder.RuleFor(prop => prop.Description, () => description);
            return builder;
        }

        public static CompanyBuilder WithDescriptionInvalidByLength(this CompanyBuilder builder, int length = 0)
        {
            var description = new Faker().Random.String(length);
            builder.WithDescriptionInvalid(description);
            return builder;
        }

        public static CompanyBuilder ByRequest(this CompanyBuilder builder, AddCompanyRequest request)
        {
            builder.WithName(request.Name)
                   .WithEmail(request.Email)
                   .WithPassword(request.Password)
                   .WithCNPJ(request.CNPJ)
                   .WithDescription(request.Description)
                   .WithLimitCancelHours(request.LimitCancelHours);
            return builder;
        }
    }
}