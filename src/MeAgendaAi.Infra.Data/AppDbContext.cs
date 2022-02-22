using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Infra.Data.Maps;
using Microsoft.EntityFrameworkCore;

namespace MeAgendaAi.Infra.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);            
        }

        public virtual DbSet<User> Users { get; set; } = default!;
        public virtual DbSet<PhysicalPerson> PhysicalPersons { get; set; } = default!;
        public virtual DbSet<Company> Companies { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(new UserMap().Configure);
            builder.Entity<PhysicalPerson>(new PhysicalPersonMap().Configure);
            builder.Entity<Company>(new CompanyMap().Configure);
        }
    }
}