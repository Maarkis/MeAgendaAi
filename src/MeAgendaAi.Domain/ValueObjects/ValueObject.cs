using FluentValidation;
using FluentValidation.Results;

namespace MeAgendaAi.Domains.ValueObjects
{
    public abstract class ValueObject
    {
        public bool Valid { get; private set; }
        public bool Invalid => !Valid;
        public ValidationResult ValidationResult { get; private set; } = new();

        public virtual bool Validate<T>(T valueObjects, AbstractValidator<T> validationRules)
        {
            ValidationResult = validationRules.Validate(valueObjects);
            return Valid = ValidationResult.IsValid;
        }

        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return obj is ValueObject valueObject && GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            const int seed = 1;
            return GetEqualityComponents().Aggregate(seed, (current, obj) =>
            {
                unchecked
                {
                    return current + (obj?.GetHashCode() ?? 0);
                }
            });
        }

        public static bool operator ==(ValueObject sourceA, ValueObject sourceB)
        {
            if (sourceA is null && sourceB is null)
                return true;

            if (sourceA is null || sourceB is null)
                return false;

            return sourceA.Equals(sourceB);
        }

        public static bool operator !=(ValueObject sourceA, ValueObject sourceB) =>
            !(sourceA == sourceB);
    }
}