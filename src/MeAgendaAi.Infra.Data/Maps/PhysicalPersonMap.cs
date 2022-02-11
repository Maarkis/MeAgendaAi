using MeAgendaAi.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeAgendaAi.Infra.Data.Maps
{
    public class PhysicalPersonMap : IEntityTypeConfiguration<PhysicalPerson>
    {
        const string ENTITY_NAME = "PHYSICAL_PERSON";        
        const string TABLE_NAME = $"TB_{ENTITY_NAME}";
        const string ID_NAME = $"PK_{TABLE_NAME}";
        const string INDEX_TABLE_NAME = $"IN_{ENTITY_NAME}";

        public void Configure(EntityTypeBuilder<PhysicalPerson> builder)
        {
            builder.ToTable(TABLE_NAME);

            builder.Property(prop => prop.Id)
                   .HasColumnName(ID_NAME);

            builder.OwnsOne(prop => prop.Name)
                   .Property(prop => prop.Name)
                   .IsRequired()
                   .HasColumnType("varchar(60)")
                   .HasMaxLength(60)
                   .HasColumnName($"NM_NAME");

            builder.OwnsOne(prop => prop.Name)
                   .Property(prop => prop.Surname)
                   .IsRequired()
                   .HasColumnType("varchar(60)")
                   .HasMaxLength(60)
                   .HasColumnName($"NM_SURNAME");

            builder.OwnsOne(prop => prop.Name)
                   .Ignore(prop => prop.Valid)
                   .Ignore(prop => prop.Invalid)
                   .Ignore(prop => prop.ValidationResult);

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

