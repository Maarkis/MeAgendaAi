﻿using AutoMapper;
using MeAgendaAi.Application.Notification;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
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
        private readonly IDistributedCacheRepository _distributedCacheRepository;
        private const string ActionType = "UserService";

        public UserService(
            IUserRepository userRepository, NotificationContext notificationContext,
            ILogger<UserService> logger, IJSONWebTokenService jsonWebTokenService,
            IMapper mapper, IDistributedCacheRepository distributedCacheRepository) : base(userRepository)
        {
            _userRepository = userRepository;
            _notificationContext = notificationContext;
            _logger = logger;
            _jsonWebTokenService = jsonWebTokenService;
            _mapper = mapper;
            _distributedCacheRepository = distributedCacheRepository;
        }

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

            var tokenJWT = _jsonWebTokenService.GenerateToken(user);

            var response = _mapper.Map<AuthenticateResponse>(user);
            response.IncludeTokenAndRefreshToken(tokenJWT.Token, tokenJWT.RefreshToken.Token);

            await _distributedCacheRepository
                .SetAsync(tokenJWT.RefreshToken.Token, user.Id.ToString(), expireIn: tokenJWT.RefreshToken.ExpiresIn);

            return response;
        }

        public async Task<AuthenticateResponse?> AuthenticateByRefreshTokenAsync(string refreshToken)
        {
            var userIdCached = await _distributedCacheRepository.GetAsync<Guid>(refreshToken);
            if(Guid.Empty.Equals(userIdCached))
            {
                _logger.LogError("[{ActionType}/AuthenticateByRefreshTokenAsync] Refresh token not found.", ActionType);
                _notificationContext.AddNotification("Resfresh Token", "Refresh token found.");
                return null;
            }

            var user = await GetByIdAsync(userIdCached);
            if (user == null)
            {
                _logger.LogError("[{ActionType}/AuthenticateByRefreshTokenAsync] User {userIdCached} not found.", ActionType, userIdCached);
                _notificationContext.AddNotification("User", "User not found.");
                return null;
            }

            var tokenJWT = _jsonWebTokenService.GenerateToken(user);

            var response = _mapper.Map<AuthenticateResponse>(user);
            response.IncludeTokenAndRefreshToken(tokenJWT.Token, tokenJWT.RefreshToken.Token);

            await _distributedCacheRepository.RemoveAsync(refreshToken);
            await _distributedCacheRepository
                .SetAsync(tokenJWT.RefreshToken.Token, user.Id.ToString(), expireIn: tokenJWT.RefreshToken.ExpiresIn);

            return response;
        }
    }
}