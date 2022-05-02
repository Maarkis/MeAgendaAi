using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Domains.Interfaces.Services;

public interface IPhysicalPersonService : IService<PhysicalPerson>
{
	Task<Guid> AddAsync(AddPhysicalPersonRequest request);
}