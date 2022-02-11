using MeAgendaAi.Domains.Entities;

namespace MeAgendaAi.Domains.Interfaces.Services
{
    public interface IUserService : IService<User>
    {
        Task<bool> HasUser(string email);
        bool SamePassword(string password, string confirmPassword);
        bool NotSamePassword(string password, string confirmPassword);
    }
}
