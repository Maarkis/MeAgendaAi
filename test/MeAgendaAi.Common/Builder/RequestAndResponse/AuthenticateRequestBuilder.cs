using AutoBogus;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Common.Builder.RequestAndResponse;

public class AuthenticateRequestBuilder : AutoFaker<AuthenticateRequest>
{
	public AuthenticateRequestBuilder() : base("pt_BR")
	{
		RuleFor(prop => prop.Password, PasswordBuilder.Generate());
		RuleFor(prop => prop.Email, faker => faker.Internet.Email());
	}
}