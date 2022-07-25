using FluentValidation;
using FluentValidation.Results;

namespace MeAgendaAi.Domains.ValueObjects;

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
}