using MeAgendaAi.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeAgendaAi.Infra.Data.Maps;

public class CompanyMap : IEntityTypeConfiguration<Company>
{
	private const string EntityName = "COMPANY";
	private const string TableName = $"TB_{EntityName}";
	private const string IndexTableName = $"IN_{EntityName}";

	public void Configure(EntityTypeBuilder<Company> builder)
	{
		builder.ToTable(TableName);

		builder.Property(prop => prop.Id);

		builder.HasIndex(prop => prop.CNPJ)
			.HasDatabaseName($"{IndexTableName}_CNPJ")
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