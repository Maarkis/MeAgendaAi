using FluentValidation;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Domains.Validators;

namespace MeAgendaAi.Application.Validators;

public class AddCompanyRequestValidator : AbstractValidator<AddCompanyRequest>
{
	private const string ErrorMessageEmpty = "Can't be empty";
	private const string ErrorMessageNull = "Can't be null";

	public AddCompanyRequestValidator()
	{
		RuleFor(prop => prop.Name).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull().WithMessage(ErrorMessageNull);
		RuleFor(prop => prop.Email).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull().WithMessage(ErrorMessageNull);
		RuleFor(prop => prop.Password).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull()
			.WithMessage(ErrorMessageNull);
		RuleFor(prop => prop.ConfirmPassword).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull()
			.WithMessage(ErrorMessageNull);
		RuleFor(prop => prop.CNPJ).NotEmpty().NotEmpty().WithMessage(ErrorMessageEmpty).NotNull()
			.WithMessage(ErrorMessageNull);
		RuleFor(prop => prop.Description).NotEmpty().WithMessage(ErrorMessageEmpty).NotNull()
			.WithMessage(ErrorMessageNull);
		RuleFor(prop => prop.LimitCancelHours).NotEmpty().WithMessage(ErrorMessageEmpty);
		RuleFor(prop => prop.Phones)
			.NotEmpty().WithMessage(ErrorMessageEmpty).NotNull().WithMessage(ErrorMessageNull);
	}
}