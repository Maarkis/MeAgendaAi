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
                        context.UseNpgsql(_configuration.GetConnectionString(TestBase.CONNECTION_STRING_NAME));
                    });

                    var serviceProvider = services.BuildServiceProvider();

                    _serviceScope = serviceProvider.CreateScope();
                    _serviceProvider = _serviceScope.ServiceProvider;
                });

            base.ConfigureWebHost(builder);
        }
    }
}