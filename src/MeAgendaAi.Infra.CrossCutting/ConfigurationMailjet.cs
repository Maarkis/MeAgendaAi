using Mailjet.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeAgendaAi.Infra.CrossCutting
{
    public static class ConfigurationMailjet
    {
        public static IServiceCollection ConfigureHttpClientMailJet(this IServiceCollection services, ConfigurationManager configuration)
        {
            ConfigureMailSender(services, configuration);
            var configurationMailJet = ConfigureOptions(services, configuration);
            services.AddHttpClient<IMailjetClient, MailjetClient>(httpClient =>
            {
                httpClient.UseBasicAuthentication(configurationMailJet.KeyApiPublic, configurationMailJet.KeyApiSecret);
            });
            return services;
        }

        private static void ConfigureMailSender(IServiceCollection services, ConfigurationManager configuration)
        {
            var config = configuration.GetSection(MailSender.SectionName);
            services.Configure<MailSender>(config);
        }

        private static ConfigurationMailJet ConfigureOptions(IServiceCollection services, ConfigurationManager configuration)
        {
            var config = configuration.GetSection(ConfigurationMailJet.SectionName);
            services.Configure<ConfigurationMailJet>(config);
            return config.Get<ConfigurationMailJet>();
        }
    }

    public class ConfigurationMailJet
    {
        public const string SectionName = "MailJet";
        public string KeyApiPublic { get; set; } = default!;
        public string KeyApiSecret { get; set; } = default!;
        public string Version { get; set; } = default!;
    }

    public class MailSender
    {
        public const string SectionName = "MailSender";
        public string FromEmail { get; set; } = default!;
        public string FromName { get; set; } = default!;
        public Dictionary<string, int> Templates { get; set; } = default!;
    }
}