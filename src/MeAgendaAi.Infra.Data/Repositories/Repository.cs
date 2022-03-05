using MeAgendaAi.Domains.Entities.Base;
using MeAgendaAi.Domains.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MeAgendaAi.Infra.Data.Repositories
{
    public abstract class Repository<T> : IRepository<T>, IDisposable where T : Entity
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<Guid> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public virtual async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FirstOrDefaultAsync(user => user.Id == id);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}