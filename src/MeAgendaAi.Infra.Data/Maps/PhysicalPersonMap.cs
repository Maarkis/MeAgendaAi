using MeAgendaAi.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeAgendaAi.Infra.Data.Maps
{
    public class PhysicalPersonMap : IEntityTypeConfiguration<PhysicalPerson>
    {
        private const string ENTITY_NAME = "PHYSICAL_PERSON";
        private const string TABLE_NAME = $"TB_{ENTITY_NAME}";
        private const string INDEX_TABLE_NAME = $"IN_{ENTITY_NAME}";

        public void Configure(EntityTypeBuilder<PhysicalPerson> builder)
        {
            builder.ToTable(TABLE_NAME);

            builder.Property(prop => prop.Id);

            builder.HasIndex(prop => prop.CPF)
                   .HasDatabaseName($"{INDEX_TABLE_NAME}_CPF")
                   .IsUnique();

            builder.Property(prop => prop.CPF)
                   .IsRequired()
                   .HasColumnType("varchar(15)")
                   .HasMaxLength(15)
                   .HasColumnName("CPF");

            builder.Property(prop => prop.RG)
                   .IsRequired()
                   .HasColumnName("RG");
        }
    }
}