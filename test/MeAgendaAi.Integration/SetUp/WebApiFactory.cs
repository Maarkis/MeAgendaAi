using MeAgendaAi.Infra.CrossCutting;
using MeAgendaAi.Infra.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Security.Authentication;

namespace MeAgendaAi.Integration.SetUp
{
    [SetUpFixture]
    public class WebApiFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly string Environment;
        public IConfiguration Configuration = default!;
        public IServiceScope ServiceScope = default!;
        public IServiceProvider ServiceProvider = default!;
        public ConfigurationRedis ConfigurationRedis = default!;
        public string ConnectionString => Configuration.GetConnectionString(TestBase.CONNECTION_STRING_DATABASE);

        public WebApiFactory(string environment = "Test") => (Environment) = (environment);

        

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseEnvironment(Environment)
                .ConfigureAppConfiguration(config =>
                {
                    Configuration = new ConfigurationBuilder()
                        .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Test.Integration.json"))
                        .Build();

                    config.AddConfiguration(Configuration);
                })
                .ConfigureServices(services =>
                {


                    var descriptor = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>((options, context) =>
                    {   
                        context.UseNpgsql(ConnectionString);
                    });

                    ConfigurationRedis = new ConfigurationRedis();
                    Configuration.GetSection(TestBase.NAME_SECTION_CACHE_DISTRIBUITED).Bind(ConfigurationRedis);
                    services.AddStackExchangeRedisCache(options =>
                    {
                        Enum.TryParse(ConfigurationRedis.SslProtocols, ignoreCase: true, out SslProtocols sslProtocols);
                        options.InstanceName = ConfigurationRedis.InstanceName;                        
                        options.ConfigurationOptions = new()
                        {
                            EndPoints = { ConfigurationRedis.Host, ConfigurationRedis.Port },
                            ConnectRetry = ConfigurationRedis.ConnectRetry,
                            ConnectTimeout = ConfigurationRedis.ConnectTimeout,
                            KeepAlive = ConfigurationRedis.KeepAlive,
                            AbortOnConnectFail = ConfigurationRedis.AbortConnect,
                            Ssl = ConfigurationRedis.Ssl,
                            SslProtocols = sslProtocols,
                            ResolveDns = ConfigurationRedis.ResolveDns
                        };
                    });
                    var serviceProvider = services.BuildServiceProvider();

                    ServiceScope = serviceProvider.CreateScope();
                    ServiceProvider = ServiceScope.ServiceProvider;
                });

            base.ConfigureWebHost(builder);
        }
    }
}