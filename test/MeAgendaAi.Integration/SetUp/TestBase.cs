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
    public class TestBase : IAsyncDisposable
    {
        public const string ConnectionStringDatabase = "AppDb";
        public const string NameSectionCacheDistribuited = "Redis";
        public const string UrlApi = "http://localhost:5000/";
        protected WebApiFactory<Program>? Server { get; private set; }
        protected HttpClient Client { get; private set; } = default!;
        public AutoMocker Mocker { get; private set; } = default!;
        protected IServiceScope ServiceScope { get; private set; } = default!;
        protected IServiceProvider ServiceProvider { get; private set; } = default!;
        protected AppDbContext DbContext { get; private set; } = default!;
        public IDistributedCache DbRedis { get; private set; } = default!;
        protected ConfigurationRedis ConfigurationRedis { get; private set; } = default!;
        public string ConnectionString { get; private set; } = default!;
        protected static IConfiguration? Configuration { get; private set; } = default!;

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
            Configuration = ServiceProvider.GetRequiredService<IConfiguration>();

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
                Server.Dispose();
                Mocker.AsDisposable().Dispose();
                GC.SuppressFinalize(this);
            }
        }

        public Uri RequisitionAssemblyFor(string entrypoint, string method = "", Dictionary<string, string>? parameters = null)
        {
            var url = new StringBuilder($"{Client.BaseAddress}api/{entrypoint}/{method}");

            if (parameters != null)
            {
                var keys = parameters.Keys;
                foreach (var key in keys)
                    url.AppendFormat($"?{key}={parameters[key]}");
            }
            return new(url.ToString());
        }
    }
}