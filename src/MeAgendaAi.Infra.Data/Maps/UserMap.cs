using MeAgendaAi.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MeAgendaAi.Infra.Data.Maps
{
    public class UserMap : BaseEntityConfigurationMap<User>
    {
        private const string EntityName = "USERS";
        private const string TableName = $"TB_{EntityName}";
        private const string IndexTableName = $"IN_{EntityName}_EMAIL";
        
        public new void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTable(TableName);

            builder.HasKey(prop => prop.Id);

            builder.OwnsOne(prop => prop.Email)
                   .HasIndex(prop => prop.Address)
                   .HasDatabaseName(IndexTableName)
                   .IsUnique();

            builder.OwnsOne(prop => prop.Email)
                   .Property(prop => prop.Address)
                   .HasColumnName("EMAIL");

            builder.OwnsOne(prop => prop.Email)
                .Ignore(prop => prop.ValidationResult)
                .Ignore(prop => prop.Valid)
                .Ignore(prop => prop.Invalid);

            builder.OwnsOne(prop => prop.Name)
                .Property(prop => prop.FirstName)
                .IsRequired(true)
                .HasColumnType("varchar(60)")
                .HasMaxLength(60)
                .HasColumnName($"NM_FIRST_NAME");

            builder.OwnsOne(prop => prop.Name)
                .Property(prop => prop.Surname)
                .IsRequired(false)
                .HasColumnType("varchar(80)")
                .HasMaxLength(80)
                .HasColumnName($"NM_LAST_NAME");

            builder.OwnsOne(prop => prop.Name)
                .Ignore(prop => prop.Valid)
                .Ignore(prop => prop.Invalid)
                .Ignore(prop => prop.ValidationResult);

            builder.Property(prop => prop.Password)
                .HasColumnName("PASS_PASSWORD")
                .IsRequired();

            builder.Property(prop => prop.IsActive)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(false)
                .HasColumnName("ACTIVE");
        }
    }
}