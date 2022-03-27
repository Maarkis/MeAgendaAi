using AutoBogus;
using FluentValidation.Results;
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

        public BaseBuilderEntity<T> WithValid(bool valid)
        {
            RuleFor(prop => prop.Valid, () => valid);
            return this;
        }

        public BaseBuilderEntity<T> WithValidationResult(ValidationResult validationResult)
        {
            RuleFor(prop => prop.ValidationResult, () => validationResult);
            return this;
        }

        public BaseBuilderEntity<T> WithCreatedAt(DateTime createAt)
        {
            RuleFor(prop => prop.CreatedAt, () => createAt);
            return this;
        }

        public BaseBuilderEntity<T> WithLastUpdatedAt(DateTime lastUpdatedAt)
        {
            RuleFor(prop => prop.LastUpdatedAt, () => lastUpdatedAt);
            return this;
        }
    }

    public class BaseBuilderValueObject<T> : AutoFaker<T> where T : ValueObjects
    {
        public BaseBuilderValueObject()
        {
            RuleFor(prop => prop.Valid, () => true);
            RuleFor(prop => prop.ValidationResult, () => new());
        }

        public BaseBuilderValueObject<T> WithValid(bool valid)
        {
            RuleFor(prop => prop.Valid, () => valid);
            return this;
        }

        public BaseBuilderValueObject<T> WithValidationResult(ValidationResult validationResult)
        {
            RuleFor(prop => prop.ValidationResult, () => validationResult);
            return this;
        }
    }
}