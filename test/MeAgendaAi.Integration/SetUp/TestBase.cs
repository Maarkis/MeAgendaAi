﻿using MeAgendaAi.Infra.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MeAgendaAi.Integration.SetUp
{
    public class TestBase : IAsyncDisposable
    {
        public static string CONNECTION_STRING_DATABASE = "AppDb";
        public static string NAME_SECTION_CACHE_DISTRIBUITED = "Redis";
        public static string URL_API = "http://localhost:5000/";
        protected WebApiFactory<Program>? _server { get; private set; }
        protected HttpClient Client { get; private set; } = default!;
        protected IServiceScope ServiceScope { get; private set; } = default!;
        protected IServiceProvider ServiceProvider { get; private set; } = default!;
        protected AppDbContext DbContext { get; private set; } = default!;
        public IDistributedCache DbRedis { get; private set; } = default!;
        protected static IConfiguration? Configuration { get; private set; } = default!;

        protected static string ConnectionString =>
            Configuration.GetConnectionString(CONNECTION_STRING_DATABASE);

        [SetUp]
        public virtual async Task SetUpAsync() => await Database.CleanAsync(ConnectionString);

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            _server = new WebApiFactory<Program>();
            Client = _server.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri(URL_API),
                AllowAutoRedirect = true
            });
            ServiceScope = _server._serviceScope;
            ServiceProvider = _server._serviceProvider;
            DbContext = ServiceProvider.GetRequiredService<AppDbContext>();
            DbRedis = ServiceProvider.GetRequiredService<IDistributedCache>();
            Configuration = ServiceProvider.GetRequiredService<IConfiguration>();

            Database.CreateDatabase(DbContext);
        }

        [OneTimeTearDown]
        public async ValueTask DisposeAsync()
        {
            if (_server != null)
            {
                Database.DeleteDatabase(DbContext);                
                Client.Dispose();
                if (ServiceScope is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync();
                else
                    ServiceScope.Dispose();
                _server.Dispose();
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