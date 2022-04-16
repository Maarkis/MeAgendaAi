using MeAgendaAi.Domains.Entities.Base;

namespace MeAgendaAi.Domains.Interfaces.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<Guid> AddAsync(T entity);

        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(Guid id);

        Task<T?> UpdateAsync(T entity);
    }
}