using AutoBogus;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Common.Builder.RequestAndResponse;

public class AuthenticateResponseBuilder : AutoFaker<AuthenticateResponse>
{
	public AuthenticateResponseBuilder() : base("pt_BR")
	{
		RuleFor(prop => prop.Id, () => Guid.NewGuid());
		RuleFor(prop => prop.Email, f => f.Internet.Email());
		RuleFor(prop => prop.CreatedAt, f => f.Date.Past(f.Random.Int()));
		RuleFor(prop => prop.LastUpdatedAt, f => f.Date.Past(f.Random.Int()));
		RuleFor(prop => prop.Token, f => f.Random.Uuid().ToString());
	}
}

public static class AuthenticateResponseBuilderExtensions
{
	public static AuthenticateResponseBuilder WithId(this AuthenticateResponseBuilder builder, Guid id)
	{
		builder.RuleFor(prop => prop.Id, () => id);
		return builder;
	}

	public static AuthenticateResponseBuilder WithEmail(this AuthenticateResponseBuilder builder, string email)
	{
		builder.RuleFor(prop => prop.Email, () => email);
		return builder;
	}

	public static AuthenticateResponseBuilder WithCreatedAt(this AuthenticateResponseBuilder builder,
		DateTime createdAt)
	{
		builder.RuleFor(prop => prop.CreatedAt, () => createdAt);
		return builder;
	}

	public static AuthenticateResponseBuilder WithLastUpdatedAt(this AuthenticateResponseBuilder builder,
		DateTime? lastUpdatedAt)
	{
		builder.RuleFor(prop => prop.LastUpdatedAt, () => lastUpdatedAt);
		return builder;
	}

	public static AuthenticateResponseBuilder WithToken(this AuthenticateResponseBuilder builder, string token)
	{
		builder.RuleFor(prop => prop.Token, () => token);
		return builder;
	}

	public static AuthenticateResponseBuilder WithRefreshToken(this AuthenticateResponseBuilder builder,
		string refreshToken)
	{
		builder.RuleFor(prop => prop.RefreshToken, () => refreshToken);
		return builder;
	}

	public static AuthenticateResponseBuilder FromUser(this AuthenticateResponseBuilder builder, User user)
	{
		builder.RuleFor(prop => prop.Id, () => user.Id);
		builder.RuleFor(prop => prop.Email, () => user.Email.Address);
		builder.RuleFor(prop => prop.CreatedAt, () => user.CreatedAt);
		builder.RuleFor(prop => prop.LastUpdatedAt, () => user.LastUpdatedAt);

		return builder;
	}
}