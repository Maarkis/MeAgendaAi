using AutoBogus;
using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Common.Builder
{
    public class BaseBuilderEntity<T> : AutoFaker<T> where T : Entity
    {
        public BaseBuilderEntity() : base("pt_BR")
        {
            RuleFor(prop => prop.Id, () => Guid.NewGuid());
            RuleFor(prop => prop.Valid, () => true);
            RuleFor(prop => prop.ValidationResult, () => new());
            RuleFor(prop => prop.CreatedAt, () => DateTime.Now);
            RuleFor(prop => prop.LastUpdatedAt, () => null);
        }
    }

    public class BaseBuilderValueObject<T> : AutoFaker<T> where T : ValueObjects
    {
        public BaseBuilderValueObject() : base("pt_BR")
        {
            RuleFor(prop => prop.Valid, () => true);
            RuleFor(prop => prop.ValidationResult, () => new());
        }
    }
}