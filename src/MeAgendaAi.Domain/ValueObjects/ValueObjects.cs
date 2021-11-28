using FluentValidation;
using FluentValidation.Results;

namespace MeAgendaAi.Domains.ValueObjects
{
    public abstract class ValueObjects
    {
        public bool Valid { get; protected set; }
        public bool Invalid => !Valid;
        public ValidationResult ValidationResult { get; protected set; } = new();

        public virtual bool Validate<T>(T valueObjects, AbstractValidator<T> validationRules)
        {
            ValidationResult = validationRules.Validate(valueObjects);
            return Valid = ValidationResult.IsValid;
        }
    }
}
