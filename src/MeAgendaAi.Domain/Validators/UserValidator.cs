using FluentValidation;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Validators.ValueObjects;

namespace MeAgendaAi.Domains.Validators;

public class UserValidator<T> : AbstractValidator<T> where T : User
{
	private const int LengthPasswordMinimum = 06;
	private const int LengthPasswordMaximum = 32;

	public UserValidator(bool includeSurname)
	{
		var phoneNumberValidator = new PhoneNumberValidator();

		RuleFor(prop => prop.Name).SetValidator(new NameValidator(includeSurname));
		RuleFor(prop => prop.Email)
			.SetValidator(new EmailValidator());
		RuleFor(prop => prop.Password)
			.NotEmpty().WithMessage("Password cannot be empty")
			.MinimumLength(LengthPasswordMinimum)
			.WithMessage($"Password must contain at least {LengthPasswordMinimum} characters")
			.MaximumLength(LengthPasswordMaximum)
			.WithMessage($"Password must contain a maximum of {LengthPasswordMaximum} characters");
		RuleForEach(prop => prop.PhoneNumbers)
			.SetValidator(phoneNumberValidator);
		RuleFor(prop => prop.PhoneNumbers)
			.NotEmpty().WithMessage("Phone number cannot be empty");
	}
}