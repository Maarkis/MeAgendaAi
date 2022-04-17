using AutoMapper;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Infra.Cryptography;
using MeAgendaAi.Infra.JWT;
using MeAgendaAi.Infra.MailJet;
using MeAgendaAí.Infra.Notification;
using Microsoft.Extensions.Logging;

namespace MeAgendaAi.Services
{
    public class UserService : Service<User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly NotificationContext _notificationContext;
        private readonly ILogger<UserService> _logger;
        private readonly IJsonWebTokenService _jsonWebTokenService;
        private readonly IMapper _mapper;
        private readonly IDistributedCacheRepository _distributedCacheRepository;
        private readonly IEmailService _emailService;

        private const int PasswordResetTokenExpirationTimeInSeconds = 3600;
        private const string ActionType = "UserService";

        public UserService(
            IUserRepository userRepository, NotificationContext notificationContext,
            ILogger<UserService> logger, IJsonWebTokenService jsonWebTokenService,
            IMapper mapper, IDistributedCacheRepository distributedCacheRepository, IEmailService emailService) : base(userRepository)
        {
            _userRepository = userRepository;
            _notificationContext = notificationContext;
            _logger = logger;
            _jsonWebTokenService = jsonWebTokenService;
            _mapper = mapper;
            _distributedCacheRepository = distributedCacheRepository;
            _emailService = emailService;
        }

        public async Task<bool> HasUser(string email) => await GetByEmailAsync(email) != null;

        public async Task<User?> GetByEmailAsync(string email) => await _userRepository.GetEmailAsync(email);

        public bool SamePassword(string password, string confirmPassword) => password.Equals(confirmPassword);

        public bool NotSamePassword(string password, string confirmPassword) => !SamePassword(password, confirmPassword);

        public async Task<AuthenticateResponse?> AuthenticateAsync(string email, string password)
        {
            var user = await GetByEmailAsync(email);
            if (user is null)
            {
                _logger.LogError("[{ActionType}/AuthenticateAsync] User {Email} not found", ActionType, email);
                _notificationContext.AddNotification("User", "User not found!");
                return null;
            }

            var isValid = Decrypt.IsValidPassword(password, user.Id.ToString(), user.Password);
            if (!isValid)
            {
                _logger.LogError("[{ActionType}/AuthenticateAsync] User {Id} Wrong your password", ActionType, user.Id);
                _notificationContext.AddNotification("User", "Wrong password.");
                return null;
            }

            var tokenJwt = _jsonWebTokenService.GenerateToken(user);

            var response = _mapper.Map<AuthenticateResponse>(user);
            response.IncludeTokenAndRefreshToken(tokenJwt.Token, tokenJwt.RefreshToken.Token);

            await _distributedCacheRepository
                .SetAsync(tokenJwt.RefreshToken.Token, user.Id.ToString(), expireIn: tokenJwt.RefreshToken.ExpiresIn);

            return response;
        }

        public async Task<AuthenticateResponse?> AuthenticateByRefreshTokenAsync(string refreshToken)
        {
            var userIdCached = await _distributedCacheRepository.GetAsync<Guid>(refreshToken);
            if (Guid.Empty.Equals(userIdCached))
            {
                _logger.LogError("[{ActionType}/AuthenticateByRefreshTokenAsync] Refresh token not found", ActionType);
                _notificationContext.AddNotification("Refresh Token", "Refresh token found");
                return null;
            }

            var user = await GetByIdAsync(userIdCached);
            if (user == null)
            {
                _logger.LogError("[{ActionType}/AuthenticateByRefreshTokenAsync] User {UserIdCached} not found", ActionType, userIdCached);
                _notificationContext.AddNotification("User", "User not found");
                return null;
            }

            var tokenJwt = _jsonWebTokenService.GenerateToken(user);

            var response = _mapper.Map<AuthenticateResponse>(user);
            response.IncludeTokenAndRefreshToken(tokenJwt.Token, tokenJwt.RefreshToken.Token);

            await _distributedCacheRepository.RemoveAsync(refreshToken);
            await _distributedCacheRepository
                .SetAsync(tokenJwt.RefreshToken.Token, user.Id.ToString(), expireIn: tokenJwt.RefreshToken.ExpiresIn);

            return response;
        }

        public async Task<string> RetrievePasswordAsync(string email)
        {
            var user = await GetByEmailAsync(email);
            if (user is null)
            {
                _logger.LogError("[{ActionType}/RetrievePasswordAsync] User {Email} not found", ActionType, email);
                _notificationContext.AddNotification("User", "User not found");
                return string.Empty;
            }

            var identificationToken = Encrypt.GenerateToken();

            await _distributedCacheRepository
                .SetAsync(identificationToken, user.Id.ToString(), expireInSeconds: PasswordResetTokenExpirationTimeInSeconds);

            var sended = await _emailService.SendPasswordRecoveryEmail(
                name: user.Name.FullName,
                email: user.Email.Address,
                token: identificationToken,
                expirationTime: PasswordResetTokenExpirationTimeInSeconds
                );

            var notSended = !sended;
            if (notSended)
            {
                _logger.LogError("[{ActionType}/RetrievePasswordAsync] Email not sent to {Email}", ActionType, email);
                _notificationContext.AddNotification("SendEmail", "Email not sent");
                return string.Empty;
            }

            _logger.LogInformation("[{ActionType}/RetrievePasswordAsync] Email successfully sent to {Email}", ActionType, email);
            return "Password recovery email sent.";
        }

        public async Task Activate(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user is null)
            {
                _logger.LogError("[{ActionType}/Activate] User {Id} not found", ActionType, id);
                _notificationContext.AddNotification("User", "User not found");
                return;
            }

            if (user.IsActive)
            {
                _logger.LogError("[{ActionType}/Activate] User {Id} is already active", ActionType, id);
                _notificationContext.AddNotification("User", "User is already active");
                return;
            }

            _logger.LogInformation("[{ActionType}/Activate] Updating user {Id}", ActionType, id);
            
            user.Active();
            await _userRepository.UpdateAsync(user);
            
            _logger.LogInformation("[{ActionType}/Activate] Updated successfully user {Id}", ActionType, id);
        }
    }
}