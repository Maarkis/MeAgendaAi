using MeAgendaAi.Domains.Entities;

namespace MeAgendaAi.Domains.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetEmailAsync(string email);
    }
}