using FluentValidation;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Application.Validators;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
	private const string ErrorMessageEmpty = "Can't be empty";
	private const string ErrorMessageNull = "Can't be null";

	public ResetPasswordRequestValidator()
	{
		RuleFor(prop => prop.Token)
			.NotEmpty().WithMessage(ErrorMessageEmpty)
			.NotNull().WithMessage(ErrorMessageNull);
		RuleFor(prop => prop.Password)
			.NotEmpty().WithMessage(ErrorMessageEmpty)
			.NotNull().WithMessage(ErrorMessageNull);
		RuleFor(prop => prop.ConfirmPassword)
			.NotEmpty().WithMessage(ErrorMessageEmpty)
			.NotNull().WithMessage(ErrorMessageNull);
	}
}