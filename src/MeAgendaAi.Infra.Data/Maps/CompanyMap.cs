using MeAgendaAi.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeAgendaAi.Infra.Data.Maps
{
    public class CompanyMap : IEntityTypeConfiguration<Company>
    {
        private const string ENTITY_NAME = "COMPANY";
        private const string TABLE_NAME = $"TB_{ENTITY_NAME}";        
        private const string INDEX_TABLE_NAME = $"IN_{ENTITY_NAME}";

        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable(TABLE_NAME);

            builder.Property(prop => prop.Id);

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
                   .HasColumnName("TIME_LIMIT_CANCEL_HOURS");

            builder.Property(prop => prop.Description)
                   .IsRequired()
                   .HasColumnType("varchar(160)")
                   .HasMaxLength(160)
                   .HasColumnName("DSC_DESCRIPTION");
        }
    }
}