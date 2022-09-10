﻿using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Domains.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MeAgendaAi.Infra.Data.Repositories;

public abstract class Repository<T> : IRepository<T>, IDisposable where T : Entity
{
	private readonly AppDbContext _context;
	private readonly DbSet<T> _dbSet;

	protected Repository(AppDbContext context)
	{
		_context = context;
		_dbSet = context.Set<T>();
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	public virtual async Task<Guid> AddAsync(T entity)
	{
		await _dbSet.AddAsync(entity);
		await _context.SaveChangesAsync();
		return entity.Id;
	}

	public virtual async Task<IEnumerable<T>> GetAllAsync()
	{
		return await _dbSet.ToListAsync();
	}

	public virtual async Task<T?> GetByIdAsync(Guid id)
	{
		return await _dbSet.FirstOrDefaultAsync(user => user.Id == id);
	}

	public async Task<T?> UpdateAsync(T entity)
	{
		_dbSet.Update(entity);
		await _context.SaveChangesAsync();
		return entity;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
			_context.Dispose();
	}
}