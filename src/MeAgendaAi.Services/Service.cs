using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;

namespace MeAgendaAi.Services
{
    public abstract class Service<T> : IService<T> where T : Entity
    {
        private IRepository<T> _repository;

        public Service(IRepository<T> repository) => _repository = repository;

        public async Task<Guid> AddAsync(T entity) => await _repository.AddAsync(entity);

        public async Task<IEnumerable<T>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<T?> GetByIdAsync(Guid id) => await _repository.GetByIdAsync(id);


    }
}
