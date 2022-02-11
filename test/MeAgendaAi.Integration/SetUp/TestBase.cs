using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Infra.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace MeAgendaAi.Integration.SetUp
{
    public class TestBase : IAsyncDisposable
    {
        public static string CONNECTION_STRING_NAME = "AppDb";
        public static string URL_API = "http://localhost:5000/";
        protected WebApiFactory<Program>? _server { get; private set; }
        protected HttpClient _client { get; private set; } = default!;
        protected IServiceScope _serviceScope { get; private set; } = default!;
        protected IServiceProvider _serviceProvider { get; private set; } = default!;
        protected AppDbContext _dbContext { get; private set; } = default!;
        protected static IConfiguration? _configuration { get; private set; } = default!;
        protected static string ConnectionString =>
            _configuration.GetConnectionString(CONNECTION_STRING_NAME);

        [SetUp]
        public virtual async Task SetUpAsync() => await Database.CleanAsync(ConnectionString);


        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            _server = new WebApiFactory<Program>();
            _client = _server.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri(URL_API),
                AllowAutoRedirect = true
            });
            _serviceScope = _server._serviceScope;
            _serviceProvider = _server._serviceProvider;
            _dbContext = _serviceProvider.GetRequiredService<AppDbContext>();
            _configuration = _serviceProvider.GetRequiredService<IConfiguration>();

            Database.CreateDatabase(_dbContext);
        }

        [OneTimeTearDown]
        public async ValueTask DisposeAsync()
        {
            if (_server != null)
            {
                Database.DeleteDatabase(_dbContext);
                _client.Dispose();
                if (_serviceScope is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync();
                else
                    _serviceScope.Dispose();
                _server.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        public Uri RequisitionAssemblyFor(string entrypoint, string method = "") => new($"{_client.BaseAddress}api/{entrypoint}/{method}");
    }
}