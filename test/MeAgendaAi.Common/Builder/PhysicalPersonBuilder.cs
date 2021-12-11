using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Common.Builder
{
    public class PhysicalPersonBuilder : BaseBuilderEntity<PhysicalPerson>
    {
        public PhysicalPersonBuilder() : base()
        {
            RuleFor(x => x.Email, () => new EmailObjectBuilder().Generate());
            RuleFor(x => x.Password, PasswordBuilder.Generate());
            RuleFor(x => x.Name, () => new NameObjectBuilder().Generate());
            RuleFor(x => x.CPF, faker => faker.Random.Int(11).ToString());
            RuleFor(x => x.RG, faker => faker.Random.Int(9).ToString());
        }
        public override PhysicalPerson Generate(string ruleSets = null!)
        {
            var entity = base.Generate(ruleSets);
            entity.Validate(entity, new PhysicalPersonValidator());
            return entity;
        }
    }

    public static class PhysicalPersonBuilderExtensions
    {
        public static PhysicalPersonBuilder WithEmail(this PhysicalPersonBuilder builder, string email)
        {
            builder.RuleFor(x => x.Email, () => new EmailObjectBuilder().WithEmail(email).Generate());
            return builder;
        }
        public static PhysicalPersonBuilder WithEmail(this PhysicalPersonBuilder builder, EmailObject email)
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
        public static PhysicalPersonBuilder WithName(this PhysicalPersonBuilder builder, NameObject name)
        {
            builder.RuleFor(x => x.Name, () => name);
            return builder;
        }
        public static PhysicalPersonBuilder WithNameAndSurname(this PhysicalPersonBuilder builder, string name, string surname)
        {
            builder.RuleFor(x => x.Name, () => new NameObjectBuilder().WithName(name).WithSurname(surname).Generate());
            return builder;
        }
        public static PhysicalPersonBuilder WithCPF(this PhysicalPersonBuilder builder, string cpf)
        {

            builder.RuleFor(x => x.CPF, () => cpf);
            return builder;
        }
        public static PhysicalPersonBuilder WithRG(this PhysicalPersonBuilder builder, string rg)
        {
            builder.RuleFor(x => x.RG, () => rg);
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
