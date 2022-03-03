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
        private readonly string _environment;
        public IConfiguration _configuration = default!;
        public IServiceScope _serviceScope = default!;
        public IServiceProvider _serviceProvider = default!;

        public WebApiFactory(string environment = "Test") => (_environment) = (environment);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseEnvironment(_environment)
                .ConfigureAppConfiguration(config =>
                {
                    _configuration = new ConfigurationBuilder()
                        .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Test.Integration.json"))
                        .Build();

                    config.AddConfiguration(_configuration);
                })
                .ConfigureServices(services =>
                {


                    var descriptor = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>((options, context) =>
                    {
                        context.UseNpgsql(_configuration.GetConnectionString(TestBase.CONNECTION_STRING_DATABASE));
                    });

                    var configurationRedis = new ConfigurationRedis();
                    _configuration.GetSection(TestBase.NAME_SECTION_CACHE_DISTRIBUITED).Bind(configurationRedis);
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
                    var serviceProvider = services.BuildServiceProvider();

                    _serviceScope = serviceProvider.CreateScope();
                    _serviceProvider = _serviceScope.ServiceProvider;
                });

            base.ConfigureWebHost(builder);
        }
    }
}