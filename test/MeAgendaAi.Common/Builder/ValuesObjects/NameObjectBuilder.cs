using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Common.Builder.ValuesObjects
{
    public class NameObjectBuilder : BaseBuilderValueObject<NameObject>
    {
        public NameObjectBuilder() : base()
        {
            RuleFor(prop => prop.Name, faker => faker.Name.FirstName());
            RuleFor(prop => prop.Surname, faker => faker.Name.LastName());
        }

        public NameObject Generate(bool validateSurname = true, string ruleSets = null!)
        {
            var valueObjets = base.Generate(ruleSets);
            valueObjets.Validate(valueObjets, new NameValidator(validateSurname));
            return valueObjets;
        }

        public override NameObject Generate(string ruleSets = null!)
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
            builder.RuleFor(prop => prop.Name, () => name);
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
            builder.RuleFor(prop => prop.Surname, () => null!);
            return builder;
        }
    }
}