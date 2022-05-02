using MeAgendaAi.Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeAgendaAi.Infra.Data.Maps;

public class PhoneNumberMap : BaseEntityConfigurationMap<PhoneNumber>
{
	private const string EntityName = "PHONE_NUMBERS";
	private const string TableName = $"TB_{EntityName}";

	public new void Configure(EntityTypeBuilder<PhoneNumber> builder)
	{
		base.Configure(builder);

		builder.ToTable(TableName);

		builder.HasKey(prop => prop.Id);

		builder.HasOne(prop => prop.User)
			.WithMany(prop => prop.PhoneNumbers)
			.HasForeignKey(prop => prop.Id);

		builder.OwnsOne(prop => prop.Contact)
			.Property(prop => prop.FirstName)
			.IsRequired(false)
			.HasColumnType("varchar(80)")
			.HasMaxLength(80)
			.HasColumnName("NM_CONTACT");

		builder.OwnsOne(prop => prop.Contact)
			.Ignore(prop => prop.Surname)
			.Ignore(prop => prop.Valid)
			.Ignore(prop => prop.Invalid)
			.Ignore(prop => prop.ValidationResult);

		builder
			.Property(prop => prop.CountryCode)
			.HasColumnName("NUM_COUNTRY_CODE")
			.HasColumnType("NUMERIC(3)")
			.IsRequired();

		builder
			.Property(prop => prop.DialCode)
			.HasColumnName("NUM_DIAL_CODE")
			.HasColumnType("NUMERIC(2)")
			.IsRequired();

		builder
			.Property(prop => prop.Number)
			.HasColumnType("varchar(9)")
			.HasColumnName("NUM_NUMBER")
			.IsRequired();

		builder
			.Property(prop => prop.Type)
			.HasColumnType("NUMERIC(1)")
			.HasColumnName("NM_TYPE")
			.HasConversion<int>()
			.IsRequired();
	}
}