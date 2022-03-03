using AutoMapper;
using MeAgendaAi.Application.Notification;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Infra.Cryptography;
using MeAgendaAi.Infra.JWT;
using Microsoft.Extensions.Logging;

namespace MeAgendaAi.Services.UserServices
{
    public class UserService : Service<User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly NotificationContext _notificationContext;
        private readonly ILogger<UserService> _logger;
        private readonly IJSONWebTokenService _jsonWebTokenService;
        private readonly IMapper _mapper;
        private const string ActionType = "UserService";

        public UserService(
            IUserRepository userRepository,
            NotificationContext notificationContext,
            ILogger<UserService> logger, IJSONWebTokenService jsonWebTokenService, IMapper mapper) : base(userRepository) =>
            (_userRepository, _notificationContext, _logger, _jsonWebTokenService, _mapper) =
            (userRepository, notificationContext, logger, jsonWebTokenService, mapper);

        public async Task<bool> HasUser(string email) => await GetByEmailAsync(email) != null;

        public async Task<User?> GetByEmailAsync(string email) => await _userRepository.GetEmailAsync(email);

        public bool SamePassword(string password, string confirmPassword) => password.Equals(confirmPassword);

        public bool NotSamePassword(string password, string confirmPassword) => !SamePassword(password, confirmPassword);

        public async Task<AuthenticateResponse?> AuthenticateAsync(string email, string password)
        {
            var user = await GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogError("[{ActionType}/AuthenticateAsync] User {email} not found.", ActionType, email);
                _notificationContext.AddNotification("User", "User not found!");
                return null;
            }

            var isValid = Decrypt.IsValidPassword(password, user.Id.ToString(), user.Password);
            if (!isValid)
            {
                _logger.LogError("[{ActionType}/AuthenticateAsync] User {Id} Wrong your password.", ActionType, user.Id);
                _notificationContext.AddNotification("User", "Wrong password.");
                return null;
            }

            var token = _jsonWebTokenService.GenerateToken(user);

            var response = _mapper.Map<AuthenticateResponse>(user);
            response.Token = token;

            return response;
        }
    }
}