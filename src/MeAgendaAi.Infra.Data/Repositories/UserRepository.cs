using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MeAgendaAi.Infra.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly DbSet<User> _dbSet;

        public UserRepository(AppDbContext context) : base(context) => _dbSet = context.Set<User>();

        public async Task<User?> GetEmailAsync(string email) =>
            await _dbSet.Where(where => where.Email.Address == email).FirstOrDefaultAsync();
    }
}