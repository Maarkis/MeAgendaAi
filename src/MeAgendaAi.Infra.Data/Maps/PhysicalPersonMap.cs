using MeAgendaAi.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeAgendaAi.Infra.Data.Maps;

public class PhysicalPersonMap : IEntityTypeConfiguration<PhysicalPerson>
{
	private const string EntityName = "PHYSICAL_PERSON";
	private const string TableName = $"TB_{EntityName}";
	private const string IndexTableName = $"IN_{EntityName}";

	public void Configure(EntityTypeBuilder<PhysicalPerson> builder)
	{
		builder.ToTable(TableName);

		builder.Property(prop => prop.Id);

		builder.HasIndex(prop => prop.CPF)
			.HasDatabaseName($"{IndexTableName}_CPF")
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