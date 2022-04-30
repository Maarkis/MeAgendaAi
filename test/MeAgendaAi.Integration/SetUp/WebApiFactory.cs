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
using System;
using System.IO;
using System.Linq;
using System.Security.Authentication;

namespace MeAgendaAi.Integration.SetUp
{
    public class WebApiFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public IServiceScope ServiceScope = default!;
        public IServiceProvider ServiceProvider = default!;
        public ConfigurationRedis ConfigurationRedis = default!;
        public readonly AutoMocker Mocker = new();
        public string ConnectionString => _configuration.GetConnectionString(TestBase.ConnectionStringDatabase);

        private readonly string _environment;
        private IConfiguration _configuration = default!;
        
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
                        context.UseNpgsql(ConnectionString);
                    });

                    ConfigurationRedis = new ConfigurationRedis();
                    _configuration.GetSection(TestBase.NameSectionCacheDistributed).Bind(ConfigurationRedis);
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

                    services.MockMailJetApi(Mocker, _configuration);

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