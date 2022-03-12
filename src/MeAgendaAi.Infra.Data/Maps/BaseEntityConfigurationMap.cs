using MeAgendaAi.Domains.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeAgendaAi.Infra.Data.Maps
{
    public abstract class BaseEntityConfigurationMap<EntityType> : IEntityTypeConfiguration<EntityType> where EntityType : Entity
    {
        public void Configure(EntityTypeBuilder<EntityType> builder)
        {
            builder.HasKey(prop => prop.Id);

            builder.Property(prop => prop.CreatedAt)
                    .IsRequired()
                    .HasColumnName("DT_CREATED_AT")
                    .HasDefaultValueSql("NOW()")
                    .ValueGeneratedOnAdd();

            builder.Property(prop => prop.LastUpdatedAt)
                   .HasColumnName("DT_LAST_UPDATED_AT")
                   .IsRequired(false)
                   .HasDefaultValueSql("NOW()")
                   .ValueGeneratedOnUpdate();

            builder.Ignore(prop => prop.Valid);
            builder.Ignore(prop => prop.Invalid);
            builder.Ignore(prop => prop.ValidationResult);
        }
    }
}