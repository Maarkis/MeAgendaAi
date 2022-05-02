using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace MeAgendaAi.Infra.CrossCutting;

public static class ConfigurationSwagger
{
	public static IServiceCollection AddAndConfigureSwaggerGen(this IServiceCollection services)
	{
		services.AddSwaggerGen(swaggerConfiguration =>
		{
			swaggerConfiguration.SwaggerDoc("v1", new OpenApiInfo
			{
				Version = "v1",
				TermsOfService = new Uri("https://github.com/Maarkis/MeAgendaAi"),
				Contact = new OpenApiContact
				{
					Name = "Jean Markis",
					Email = "jeanmarkis85@gmail.com",
					Url = new Uri("https://github.com/Maarkis/MeAgendaAi")
				},
				License = new OpenApiLicense
				{
					Name = "Use under LICX",
					Url = new Uri("https://example.com/license")
				}
			});

			swaggerConfiguration.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = "JSON Web Token",
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey
			});

			swaggerConfiguration.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Id = "Bearer",
							Type = ReferenceType.SecurityScheme
						}
					},
					new List<string>()
				}
			});
		});

		return services;
	}
}