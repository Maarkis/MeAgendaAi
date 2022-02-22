using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Domains.Interfaces.Services
{
    public interface IUserService : IService<User>
    {
        Task<AuthenticateResponse?> AuthenticateAsync(string email, string password);

        Task<AuthenticateResponse?> AuthenticateByRefreshTokenAsync(string refreshToken);

        Task<User?> GetByEmailAsync(string email);

        Task<bool> HasUser(string email);

        bool NotSamePassword(string password, string confirmPassword);

        bool SamePassword(string password, string confirmPassword);
    }
}