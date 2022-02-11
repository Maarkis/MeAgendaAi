using MeAgendaAi.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeAgendaAi.Infra.Data.Maps
{
    public class CompanyMap : IEntityTypeConfiguration<Company>
    {
        const string ENTITY_NAME = "COMPANY";
        const string TABLE_NAME = $"TB_{ENTITY_NAME}";
        const string ID_NAME = $"PK_{TABLE_NAME}";
        const string INDEX_TABLE_NAME = $"IN_{ENTITY_NAME}";
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable(TABLE_NAME);

            builder.Property(prop => prop.Id);

            builder.OwnsOne(prop => prop.Name)
                   .Property(prop => prop.Name)
                   .IsRequired()
                   .HasColumnType("varchar(60)")
                   .HasMaxLength(60)
                   .HasColumnName($"NM_NAME");

            builder.OwnsOne(prop => prop.Name)
                   .Ignore(prop => prop.Valid)
                   .Ignore(prop => prop.Invalid)
                   .Ignore(prop => prop.Surname)
                   .Ignore(prop => prop.ValidationResult);

            builder.HasIndex(prop => prop.CNPJ)
                   .HasDatabaseName($"{INDEX_TABLE_NAME}_CNPJ")
                   .IsUnique();

            builder.Property(prop => prop.CNPJ)
                   .IsRequired()
                   .HasColumnType("varchar(15)")
                   .HasMaxLength(15)
                   .HasColumnName("CNPJ");

            builder.Property(prop => prop.LimitCancelHours)
                   .IsRequired()
                   .HasColumnType("smallint")
                   .HasDefaultValue(0)
                   .HasColumnName("TIME_LIMITCANCELHOURS");

            builder.Property(prop => prop.Description)
                   .IsRequired()
                   .HasColumnType("varchar(160)")
                   .HasMaxLength(160)
                   .HasColumnName("DSC_DESCRIPTION");
        }
    }
}
