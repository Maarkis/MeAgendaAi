using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MeAgendaAi.Infra.Data.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private DbSet<Company> _dbSet;
        public CompanyRepository(AppDbContext context) : base(context) =>
            _dbSet = context.Set<Company>();
    }
}
