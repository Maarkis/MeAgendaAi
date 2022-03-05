using FluentValidation;
using FluentValidation.Results;

namespace MeAgendaAi.Domains.Entities.Base
{
    /// <summary>
    /// Represent Abstract <c>Entity</c> class.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Represent Id.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Represent Created At.
        /// </summary>
        public DateTime CreatedAt { get; protected set; }

        /// <summary>
        /// Represent Last updated at.
        /// </summary>
        public DateTime? LastUpdatedAt { get; protected set; }

        /// <summary>
        /// Represent whether entity is valid.
        /// </summary>
        public bool Valid { get; protected set; }

        /// <summary>
        /// Represent whether entity is invalid.
        /// </summary>
        public bool Invalid => !Valid;

        /// <summary>
        /// Store all entity errors.
        /// </summary>
        public ValidationResult ValidationResult { get; protected set; }

        public Entity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            LastUpdatedAt = null;
            ValidationResult = new();
        }

        /// <summary>
        /// Method to validate the class.
        /// </summary>
        /// <typeparam name="T">The type stored by entity and validator.</typeparam>
        /// <param name="entity">Class to be validated.</param>
        /// <param name="validationRules">Class validator.</param>
        /// <returns>
        ///     <c>true</c> if a entity is valid.
        ///     <c>false</c> if a entity is invalid.
        /// </returns>
        protected virtual bool Validate<T>(T entity, AbstractValidator<T> validationRules)
        {
            ValidationResult = validationRules.Validate(entity);
            return Valid = ValidationResult.IsValid;
        }
    }
}