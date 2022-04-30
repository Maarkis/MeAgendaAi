using CsvHelper.Configuration;
using MeAgendaAi.Domains.Entities;

namespace MeAgendaAi.Services.CSVMaps;

public sealed class PhysicalPersonsMap : ClassMap<PhysicalPerson>
{
	public PhysicalPersonsMap()
	{
		Map(m => m.Id).Name("User Code");
		Map(m => m.Name.FullName).Name("Name");
		Map(m => m.Email.Address).Name("Email");
		Map(m => m.Rg).Name("Rg");
		Map(m => m.Cpf).Name("Cpf");
		Map(m => m.IsActive).Name("Active");
	}
}