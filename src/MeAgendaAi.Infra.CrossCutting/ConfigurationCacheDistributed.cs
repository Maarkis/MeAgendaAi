using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Infra.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Authentication;

namespace MeAgendaAi.Infra.CrossCutting
{
    public static class ConfigurationCacheDistributed
    {
        public static IServiceCollection ConfigureCacheDistributed(this IServiceCollection services, ConfigurationManager configuration)
        {
            var configurationRedis = new ConfigurationRedis();
            configuration.GetSection("Redis").Bind(configurationRedis);
            services.AddStackExchangeRedisCache(options =>
            {
                Enum.TryParse(configurationRedis.SslProtocols, ignoreCase: true, out SslProtocols sslProtocols);
                options.InstanceName = configurationRedis.InstanceName;
                options.ConfigurationOptions = new()
                {
                    EndPoints = { configurationRedis.Host, configurationRedis.Port },
                    ConnectRetry = configurationRedis.ConnectRetry,
                    ConnectTimeout = configurationRedis.ConnectTimeout,
                    KeepAlive = configurationRedis.KeepAlive,
                    AbortOnConnectFail = configurationRedis.AbortConnect,
                    Ssl = configurationRedis.Ssl,
                    SslProtocols = sslProtocols,
                    ResolveDns = configurationRedis.ResolveDns
                };
            });
            return services;
        }

        public static IServiceCollection ConfigureCacheDistributedService(this IServiceCollection services)
        {
            services.AddScoped<IDistributedCacheRepository, DistributedCacheRepository>();
            return services;
        }
    }

    public class ConfigurationRedis
    {
        public string Host { get; set; } = default!;
        public string Port { get; set; } = default!;
        public string InstanceName { get; set; } = default!;
        public int ConnectRetry { get; set; }
        public int ConnectTimeout { get; set; }
        public int KeepAlive { get; set; }
        public bool AbortConnect { get; set; }
        public bool Ssl { get; set; }
        public bool ResolveDns { get; set; }
        public string SslProtocols { get; set; } = default!;
    }
}