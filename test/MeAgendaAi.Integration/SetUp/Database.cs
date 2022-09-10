using System.Threading.Tasks;
using MeAgendaAi.Infra.Data;
using Npgsql;
using Respawn;
using Respawn.Graph;
using StackExchange.Redis;

namespace MeAgendaAi.Integration.SetUp;

public static class Database
{
	private static readonly Checkpoint _checkpoint = new()
	{
		SchemasToInclude = new[] { "public" },
		TablesToIgnore = new[] { new Table("_EFMigrationsHistory") },
		DbAdapter = DbAdapter.Postgres
	};

	public static void CreateDatabase(AppDbContext dbContext)
	{
		dbContext.Database.EnsureCreated();
	}

	public static void DeleteDatabase(AppDbContext dbContext)
	{
		dbContext.Database.EnsureDeleted();
	}

	public static async Task CleanAsync(string connectionString)
	{
		await using var dbConnection = new NpgsqlConnection(connectionString);
		await dbConnection.OpenAsync();
		await _checkpoint.Reset(dbConnection);
		await dbConnection.CloseAsync();
	}
}

public static class DatabaseRedis
{
	public static async Task CleanAsync(string host, string port)
	{
		var redis = await ConnectionMultiplexer.ConnectAsync($"{host},{port},allowAdmin=true");
		var server = redis.GetServer($"{host}:{port}");
		await server.FlushDatabaseAsync();
	}
}