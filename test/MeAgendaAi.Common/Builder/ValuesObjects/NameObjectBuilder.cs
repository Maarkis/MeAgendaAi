using MeAgendaAi.Domains.Validators.ValueObjects;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Common.Builder.ValuesObjects
{
    public class NameObjectBuilder : BaseBuilderValueObject<Name>
    {
        public NameObjectBuilder() : base()
        {
            RuleFor(prop => prop.FirstName, faker => faker.Name.FirstName());
            RuleFor(prop => prop.Surname, faker => faker.Name.LastName());
        }

        public Name Generate(bool validateSurname = true, string ruleSets = null!)
        {
            var valueObjets = base.Generate(ruleSets);
            valueObjets.Validate(valueObjets, new NameValidator(validateSurname));
            return valueObjets;
        }

        public override Name Generate(string ruleSets = null!)
        {
            var valueObjets = base.Generate(ruleSets);
            valueObjets.Validate(valueObjets, new NameValidator());
            return valueObjets;
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
            builder.RuleFor(prop => prop.Surname, string.Empty);
            return builder;
        }
    }
}