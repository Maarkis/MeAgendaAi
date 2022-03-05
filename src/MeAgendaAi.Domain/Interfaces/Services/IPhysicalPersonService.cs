using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Domains.Interfaces.Services
{
    /// <summary>
    /// Agreement for physical person service.
    /// </summary>
    public interface IPhysicalPersonService : IService<PhysicalPerson>
    {
        /// <summary>
        /// Add physical person.
        /// </summary>
        /// <param name="request">Object with information for adding a physical person.</param>
        /// <returns>
        /// Return <c>Id</c> of physical person.
        /// </returns>
        Task<Guid> AddAsync(AddPhysicalPersonRequest request);
    }
}