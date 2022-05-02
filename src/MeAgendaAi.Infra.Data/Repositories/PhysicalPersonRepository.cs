using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;

namespace MeAgendaAi.Infra.Data.Repositories;

public class PhysicalPersonRepository : Repository<PhysicalPerson>, IPhysicalPersonRepository
{
	public PhysicalPersonRepository(AppDbContext context) : base(context)
	{
	}
}