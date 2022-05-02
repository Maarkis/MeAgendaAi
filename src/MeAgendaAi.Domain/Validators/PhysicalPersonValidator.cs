using FluentValidation;
using MeAgendaAi.Domains.Entities;

namespace MeAgendaAi.Domains.Validators;

public class PhysicalPersonValidator : UserValidator<PhysicalPerson>
{
	public PhysicalPersonValidator() : base(true)
	{
		RuleFor(prop => prop.CPF)
			.NotNull()
			.WithMessage("CPF cannot be null")
			.NotEmpty()
			.WithMessage("CPF cannot be empty");
		RuleFor(prop => prop.RG)
			.NotNull()
			.WithMessage("RG cannot be null")
			.NotEmpty()
			.WithMessage("RG cannot be empty");
	}
}