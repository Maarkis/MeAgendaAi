using MeAgendaAi.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;

namespace MeAgendaAi.Infra.Data.Maps
{
    public class UserMap : BaseEntityConfigurationMap<User>
    {
        const string ENTITY_NAME = "USERS";
        const string TABLE_NAME = $"TB_{ENTITY_NAME}";
        const string ID_NAME = $"PK_{TABLE_NAME}";
        const string INDEX_TABLE_NAME = $"IN_{ENTITY_NAME}_EMAIL";
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

            builder.OwnsOne(prop => prop.Email).Ignore(prop => prop.ValidationResult);
            builder.OwnsOne(prop => prop.Email).Ignore(prop => prop.Valid);
            builder.OwnsOne(prop => prop.Email).Ignore(prop => prop.Invalid);

            builder.Property(prop => prop.Password)
                   .HasColumnName("PASS_PASSWORD")
                   .IsRequired();
        }
    }
}
