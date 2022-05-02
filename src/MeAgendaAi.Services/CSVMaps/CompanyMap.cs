using CsvHelper.Configuration;
using MeAgendaAi.Domains.Entities;

namespace MeAgendaAi.Services.CSVMaps;

public sealed class CompanyMap : ClassMap<Company>
{
	public CompanyMap()
	{
		Map(m => m.Id).Name("User Code");
		Map(m => m.Name.FullName).Name("Name");
		Map(m => m.Email.Address).Name("Email");
		Map(m => m.CNPJ).Name("CNPJ");
		Map(m => m.Description).Name("Description");
		Map(m => m.LimitCancelHours).Name("Limit cancel hours");
	}
}