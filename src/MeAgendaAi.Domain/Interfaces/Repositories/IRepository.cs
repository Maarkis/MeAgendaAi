using MeAgendaAi.Domains.Entities.Base;

namespace MeAgendaAi.Domains.Interfaces.Repositories
{
    /// <summary>
    /// Agreement for Repository.
    /// </summary>
    /// <typeparam name="T">The type stored by repository.</typeparam>
    public interface IRepository<T> where T : Entity
    {
        /// <summary>
        /// Add an entity of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        /// <returns>
        /// Returns the entity <c>Id</c>.
        /// </returns>
        Task<Guid> AddAsync(T entity);

        /// <summary>
        /// Gets a total list of entity of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>
        /// Returns a list of entity of type <typeparamref name="T"/>.
        /// </returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Get an entity of type <typeparamref name="T"/> by Id.
        /// </summary>
        /// <param name="id">Id to be fetched.</param>
        /// <returns>
        /// Return an entity of type <typeparamref name="T"/> if <c>found</c>. Return <c>default(<typeparamref name="T"/>)</c> if not found.
        /// </returns>
        Task<T?> GetByIdAsync(Guid id);
    }
}