using MeAgendaAi.Domains.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeAgendaAi.Infra.Data.Maps;

public abstract class BaseEntityConfigurationMap<TEntityType> : IEntityTypeConfiguration<TEntityType>
	where TEntityType : Entity
{
	public void Configure(EntityTypeBuilder<TEntityType> builder)
	{
		builder.HasKey(prop => prop.Id);

		builder.Property(prop => prop.CreatedAt)
			.IsRequired()
			.HasColumnName("DT_CREATED_AT");

		builder.Property(prop => prop.LastUpdatedAt)
			.HasColumnName("DT_LAST_UPDATED_AT")
			.IsRequired(false);

		builder.Ignore(prop => prop.Valid);
		builder.Ignore(prop => prop.Invalid);
		builder.Ignore(prop => prop.ValidationResult);
	}
}