using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;

namespace MeAgendaAi.Services.UserServices
{
    public class UserService : Service<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) : base(userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> HasUser(string email) => await _userRepository.GetByEmail(email) != null;
        public bool SamePassword(string password, string confirmPassword) => password.Equals(confirmPassword);
        public bool NotSamePassword(string password, string confirmPassword) => !SamePassword(password, confirmPassword);
    }

}
