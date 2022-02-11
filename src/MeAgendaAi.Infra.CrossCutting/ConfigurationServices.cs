using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Services;
using MeAgendaAi.Services.UserServices;
using Microsoft.Extensions.DependencyInjection;

namespace MeAgendaAi.Infra.CrossCutting
{
    public static class ConfigurationServices
    {
        public static IServiceCollection ConfigureServicesDependecies(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPhysicalPersonService, PhysicalPersonService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IReport, ReportService>();
            return services;
        }
    }
}
