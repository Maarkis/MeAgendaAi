using FluentValidation;
using FluentValidation.Results;

namespace MeAgendaAi.Domains.ValueObjects
{
    /// <summary>
    /// Represents a value object.
    /// </summary>
    public abstract class ValueObjects
    {
        /// <summary>
        /// <c>True</c> if is valid. <c>False</c> if is invalid.
        /// </summary>
        public bool Valid { get; protected set; }

        /// <summary>
        /// <c>True</c> if is invalid. <c>False</c> if is valid.
        /// </summary>
        public bool Invalid => !Valid;

        /// <summary>
        /// Store all value object errors.
        /// </summary>
        public ValidationResult ValidationResult { get; protected set; } = new();

        /// <summary>
        /// Validate the value object.
        /// </summary>
        /// <typeparam name="T">The type stored by value object and validator.</typeparam>
        /// <param name="valueObjects">Value object.</param>
        /// <param name="validationRules">Class validator.</param>
        /// <returns>
        ///     <c>true</c> if a entity is valid.
        ///     <c>false</c> if a entity is invalid.
        /// </returns>
        public virtual bool Validate<T>(T valueObjects, AbstractValidator<T> validationRules)
        {
            ValidationResult = validationRules.Validate(valueObjects);
            return Valid = ValidationResult.IsValid;
        }
    }
}