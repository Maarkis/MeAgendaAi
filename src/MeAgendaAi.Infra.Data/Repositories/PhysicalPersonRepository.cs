using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MeAgendaAi.Infra.Data.Repositories
{
    public class PhysicalPersonRepository : Repository<PhysicalPerson>, IPhysicalPersonRepository
    {
        private DbSet<PhysicalPerson> _dbSet;
        public PhysicalPersonRepository(AppDbContext context): base(context) => 
            _dbSet = context.Set<PhysicalPerson>();
        
    }
}
