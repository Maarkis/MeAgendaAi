using MeAgendaAi.Infra.CrossCutting;
using MeAgendaAi.Infra.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MeAgendaAi.Integration.SetUp
{
    public abstract class TestBase : IAsyncDisposable
    {
        public const string ConnectionStringDatabase = "AppDb";
        public const string NameSectionCacheDistributed = "Redis";
        protected const string UrlApi = "http://localhost:5000/api";
        protected abstract string EntryPoint { get; }
        private WebApiFactory<Program>? Server { get; set; }
        protected HttpClient Client { get; private set; } = default!;
        protected AutoMocker Mocker { get; private set; } = default!;
        private IServiceScope ServiceScope { get; set; } = default!;
        private IServiceProvider ServiceProvider { get; set; } = default!;
        protected AppDbContext DbContext { get; private set; } = default!;
        protected IDistributedCache DbRedis { get; private set; } = default!;
        private ConfigurationRedis ConfigurationRedis { get; set; } = default!;
        private string ConnectionString { get; set; } = default!;

        [SetUp]
        public virtual async Task SetUpAsync()
        {
            await Database.CleanAsync(ConnectionString);
            await DatabaseRedis.CleanAsync(ConfigurationRedis.Host, ConfigurationRedis.Port);
        }

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            Server = new WebApiFactory<Program>();
            Client = Server.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri(UrlApi),
                AllowAutoRedirect = true
            });

            Mocker = Server.Mocker;
            ConfigurationRedis = Server.ConfigurationRedis;
            ConnectionString = Server.ConnectionString;
            ServiceScope = Server.ServiceScope;
            ServiceProvider = Server.ServiceProvider;
            DbContext = ServiceProvider.GetRequiredService<AppDbContext>();
            DbRedis = ServiceProvider.GetRequiredService<IDistributedCache>();
            ServiceProvider.GetRequiredService<IConfiguration>();

            Database.CreateDatabase(DbContext);
        }

        [OneTimeTearDown]
        public async ValueTask DisposeAsync()
        {
            if (Server != null)
            {
                Database.DeleteDatabase(DbContext);
                Client.Dispose();
                if (ServiceScope is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync();
                else
                    ServiceScope.Dispose();
                await Server.DisposeAsync();
                Mocker.AsDisposable().Dispose();
                GC.SuppressFinalize(this);
            }
        }

        protected Uri RequisitionAssemblyFor(string entrypoint, string method = "", Dictionary<string, string>? parameters = null)
        {
            var url = new StringBuilder($"{Client.BaseAddress}/{entrypoint}/{method}");

            if (parameters == null) 
                return new(url.ToString());
            
            var keys = parameters.Keys;
            foreach (var key in keys)
                url.AppendFormat($"?{key}={parameters[key]}");
            return new(url.ToString());
        }
    }
}