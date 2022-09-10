using MeAgendaAi.Infra.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MeAgendaAi.Infra.CrossCutting;

public static class ConfigurationJwt
{
	public static IServiceCollection ConfigureJWT(this IServiceCollection services, ConfigurationManager configuration)
	{
		services.AddSingleton<IJsonWebTokenService, JwtService>();

		var tokenConfiguration = CreateTokenCofiguration(services, configuration);
		var signingConfiguration = CreateSigningConfiguration(services);

		AddAuthentication(services, tokenConfiguration, signingConfiguration);
		AddAuthorization(services);

		return services;
	}

	public static SigningConfiguration CreateSigningConfiguration(IServiceCollection services)
	{
		var signingConfiguration = new SigningConfiguration();
		services.AddSingleton(signingConfiguration);

		return signingConfiguration;
	}

	public static TokenConfiguration CreateTokenCofiguration(IServiceCollection services,
		ConfigurationManager configuration)
	{
		var tokenConfiguration = configuration.GetSection(TokenConfiguration.SectionName);
		services.Configure<TokenConfiguration>(tokenConfiguration);

		return tokenConfiguration.Get<TokenConfiguration>();
	}

	private static void AddAuthentication(IServiceCollection services, TokenConfiguration tokenConfiguration,
		SigningConfiguration signingConfiguration)
	{
		services.AddAuthentication(authenticationOptions =>
		{
			authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(jwtBearerOptions =>
		{
			jwtBearerOptions.RequireHttpsMetadata = false;
			jwtBearerOptions.SaveToken = true;

			jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
			{
				IssuerSigningKey = signingConfiguration.Key,
				ValidateAudience = true,
				ValidAudience = tokenConfiguration.Audience,
				ValidateIssuer = true,
				ValidIssuer = tokenConfiguration.Issuer,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};

			jwtBearerOptions.Events = new JwtBearerEvents
			{
				OnAuthenticationFailed = context =>
				{
					if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
						context.Response.Headers.Add("Token-Expired", "true");
					return Task.CompletedTask;
				}
			};
		});
	}

	private static void AddAuthorization(IServiceCollection services)
	{
		services.AddAuthorization(auth =>
		{
			auth.AddPolicy("Administrador", new AuthorizationPolicyBuilder()
				.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
				.RequireAuthenticatedUser()
				.Build());
		});
	}
}