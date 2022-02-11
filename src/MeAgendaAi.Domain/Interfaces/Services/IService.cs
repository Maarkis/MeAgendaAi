using MeAgendaAi.Domains.Entities.Base;

namespace MeAgendaAi.Domains.Interfaces.Services
{
    public interface IService<T> where T : Entity
    {
        Task<Guid> AddAsync(T entity);
    }
}
