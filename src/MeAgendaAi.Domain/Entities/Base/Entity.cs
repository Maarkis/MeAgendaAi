﻿using FluentValidation;
using FluentValidation.Results;

namespace MeAgendaAi.Domains.Entities.Base;

public abstract class Entity
{
	protected Entity()
	{
		Id = Guid.NewGuid();
		CreatedAt = DateTime.Now;
		LastUpdatedAt = null;
		ValidationResult = new ValidationResult();
	}

	public Guid Id { get; protected set; }
	public DateTime CreatedAt { get; protected set; }
	public DateTime? LastUpdatedAt { get; protected set; }
	public bool Valid { get; protected set; }
	public bool Invalid => !Valid;
	public ValidationResult ValidationResult { get; protected set; }

	protected virtual bool Validate<T>(T entity, AbstractValidator<T> validationRules)
	{
		ValidationResult = validationRules.Validate(entity);
		return Valid = ValidationResult.IsValid;
	}

	protected virtual void UpdatedAt()
	{
		LastUpdatedAt = DateTime.Now;
	}
}