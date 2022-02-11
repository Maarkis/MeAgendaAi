using MeAgendaAi.Domains.Entities.Base;

namespace MeAgendaAi.Domains.Interfaces.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<Guid> AddAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
