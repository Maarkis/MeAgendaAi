using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Infra.Data;
using MeAgendaAi.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MeAgendaAi.Infra.CrossCutting;

public static class ConfigurationDatabase
{
	public static IServiceCollection ConfigureDatabase(this IServiceCollection services, string connectionString)
	{
		services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
		return services;
	}

	public static IServiceCollection ConfigureDatabaseDependencies(this IServiceCollection services)
	{
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IPhysicalPersonRepository, PhysicalPersonRepository>();
		services.AddScoped<ICompanyRepository, CompanyRepository>();
		return services;
	}
}