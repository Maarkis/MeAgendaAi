using FluentValidation;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Validators.ValueObjects;

namespace MeAgendaAi.Domains.Validators;

public sealed class PhoneNumberValidator : AbstractValidator<PhoneNumber>
{
	public PhoneNumberValidator()
	{
		RuleFor(prop => prop.CountryCode)
			.NotEmpty()
			.WithMessage("Country Code can't be empty");

		RuleFor(prop => prop.DialCode)
			.NotEmpty()
			.WithMessage("Dial Code can't be empty");

		RuleFor(prop => prop.Number)
			.NotEmpty()
			.WithMessage("Number can't be empty")
			.NotNull()
			.WithMessage("Number can't be null");

		RuleFor(prop => prop.Type)
			.IsInEnum()
			.WithMessage("Phone type entered incorrectly");

		RuleFor(prop => prop.Contact)
			.SetValidator(new NameValidator(false)!)
			.When(prop => prop.Contact is not null);
	}
}