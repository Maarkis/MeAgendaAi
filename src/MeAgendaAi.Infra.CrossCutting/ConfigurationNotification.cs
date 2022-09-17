using MeAgendaAi.Infra.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace MeAgendaAi.Infra.CrossCutting;

public static class ConfigurationNotification
{
	public static IServiceCollection ConfigureNotification(this IServiceCollection services)
	{
		services.AddScoped<NotificationContext>();
		return services;
	}

	public static IMvcBuilder ConfigureMiddlewareNotification(this IServiceCollection services)
	{
		return services.AddMvc(options => options.Filters.Add<NotificationMiddleware>());
	}
}