using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;

namespace MeAgendaAi.Services;

public abstract class Service<T> : IService<T> where T : Entity
{
	private readonly IRepository<T> _repository;

	protected Service(IRepository<T> repository)
	{
		_repository = repository;
	}

	public async Task<Guid> AddAsync(T entity)
	{
		return await _repository.AddAsync(entity);
	}

	public async Task<IEnumerable<T>> GetAllAsync()
	{
		return await _repository.GetAllAsync();
	}

	public async Task<T?> GetByIdAsync(Guid id)
	{
		return await _repository.GetByIdAsync(id);
	}
}