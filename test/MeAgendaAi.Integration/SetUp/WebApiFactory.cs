using MeAgendaAi.Infra.CrossCutting;
using MeAgendaAi.Infra.Data;
using MeAgendaAi.Infra.MailJet;
using MeAgendaAi.Infra.MailJet.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Moq.AutoMock;
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
        public AutoMocker Mocker = new();
        public string ConnectionString => Configuration.GetConnectionString(TestBase.ConnectionStringDatabase);

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
                    Configuration.GetSection(TestBase.NameSectionCacheDistribuited).Bind(ConfigurationRedis);
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

                    services.MockMailJetApi(Mocker, Configuration);

                    var serviceProvider = services.BuildServiceProvider();

                    ServiceScope = serviceProvider.CreateScope();
                    ServiceProvider = ServiceScope.ServiceProvider;
                });

            base.ConfigureWebHost(builder);
        }
    }

    public static class MockService
    {
        public static IServiceCollection MockMailJetApi(this IServiceCollection services, AutoMocker mocker, IConfiguration configuration)
        {
            var mailSender = new MailSender();
            configuration.GetSection(MailSender.SectionName).Bind(mailSender);
            var descriptor = services.SingleOrDefault(service => service.ServiceType == typeof(IEmailService));
            if (descriptor != null)
                services.Remove(descriptor);

            mocker.GetMock<IOptions<MailSender>>()
                .Setup(setup => setup.Value)
                .Returns(mailSender);

            var mockMailjetClient = mocker.CreateInstance<EmailService>();

            services.TryAddScoped<IEmailService>(serviceProvider => mockMailjetClient);

            return services;
        }
    }
}