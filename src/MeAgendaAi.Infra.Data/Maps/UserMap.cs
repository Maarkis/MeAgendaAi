using MeAgendaAi.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeAgendaAi.Infra.Data.Maps
{
    public class UserMap : BaseEntityConfigurationMap<User>
    {
        private const string ENTITY_NAME = "USERS";
        private const string TABLE_NAME = $"TB_{ENTITY_NAME}";
        private const string INDEX_TABLE_NAME = $"IN_{ENTITY_NAME}_EMAIL";

        public new void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTable(TABLE_NAME);

            builder.HasKey(prop => prop.Id);

            builder.OwnsOne(prop => prop.Email)
                   .HasIndex(prop => prop.Email)
                   .HasDatabaseName(INDEX_TABLE_NAME)
                   .IsUnique();

            builder.OwnsOne(prop => prop.Email)
                   .Property(prop => prop.Email)
                   .HasColumnName("EMAIL");

            builder.OwnsOne(prop => prop.Email)
                .Ignore(prop => prop.ValidationResult)
                .Ignore(prop => prop.Valid)
                .Ignore(prop => prop.Invalid);

            builder.OwnsOne(prop => prop.Name)
                .Property(prop => prop.Name)
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
        }
    }
}