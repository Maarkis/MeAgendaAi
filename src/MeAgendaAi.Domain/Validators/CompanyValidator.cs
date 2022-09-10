using FluentValidation;
using MeAgendaAi.Domains.Entities;

namespace MeAgendaAi.Domains.Validators;

public class CompanyValidator : UserValidator<Company>
{
	public CompanyValidator() : base(false)
	{
		RuleFor(prop => prop.CNPJ)
			.NotNull()
			.WithMessage("CNPJ cannot be null")
			.NotEmpty()
			.WithMessage("CNPJ cannot be empty");
		RuleFor(prop => prop.LimitCancelHours)
			.NotEmpty().WithMessage("Limit cancel hours cannot be empty")
			.GreaterThan(0).WithMessage("Must be greater than zero");
		RuleFor(prop => prop.Description)
			.NotEmpty().WithMessage("Description cannot be empty")
			.NotNull().WithMessage("Description cannot be null")
			.MinimumLength(3).WithMessage("Description must contain at least 3 characters")
			.MaximumLength(160).WithMessage("Description must contain a maximum of 160 characters");
	}
}