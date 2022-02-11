using MeAgendaAi.Domains.Entities;

namespace MeAgendaAi.Domains.Interfaces.Services
{
    public interface IUserService : IService<User>
    {
        Task<bool> HasUser(string email);
    }
} 
