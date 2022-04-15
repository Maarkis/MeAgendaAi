using Mailjet.Client;
using MeAgendaAi.Infra.MailJet;
using MeAgendaAi.Infra.MailJet.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeAgendaAi.Infra.CrossCutting
{
    public static class ConfigurationMailjet
    {
        public static IServiceCollection ConfigureServiceAndHttpClientMailJet(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
                .ConfigureEmailService()
                .ConfigureMailSender(configuration);

            var configurationMailJet = ConfigureOptions(services, configuration);
            services.AddHttpClient<IMailjetClient, MailjetClient>(httpClient =>
            {
                httpClient.UseBasicAuthentication(configurationMailJet.KeyApiPublic, configurationMailJet.KeyApiSecret);
            });

            return services;
        }

        private static IServiceCollection ConfigureEmailService(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }

        private static IServiceCollection ConfigureMailSender(this IServiceCollection services, ConfigurationManager configuration)
        {
            var config = configuration.GetSection(MailSender.SectionName);
            services.Configure<MailSender>(config);

            return services;
        }

        private static ConfigurationMailJet ConfigureOptions(IServiceCollection services, ConfigurationManager configuration)
        {
            var config = configuration.GetSection(ConfigurationMailJet.SectionName);
            services.Configure<ConfigurationMailJet>(config);
            return config.Get<ConfigurationMailJet>();
        }
    }
}