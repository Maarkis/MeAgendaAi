using MeAgendaAi.Infra.Data;
using Npgsql;
using Respawn;
using System.Threading.Tasks;

namespace MeAgendaAi.Integration.SetUp
{
    public static class Database
    {
        private static readonly Checkpoint _checkpoint = new()
        {
            SchemasToInclude = new[] { "public" },
            TablesToIgnore = new[] { "_EFMigrationsHistory" },
            DbAdapter = DbAdapter.Postgres
        };
        public static void CreateDatabase(AppDbContext dbContext) => dbContext.Database.EnsureCreated();
        public static void DeleteDatabase(AppDbContext dbContext) => dbContext.Database.EnsureDeleted();

        public static async Task CleanAsync(string connectionString)
        {
            using var dbConnection = new NpgsqlConnection(connectionString);
            await dbConnection.OpenAsync();
            await _checkpoint.Reset(dbConnection);
            await dbConnection.CloseAsync();
        }
    }
}
