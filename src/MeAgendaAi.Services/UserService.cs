using AutoMapper;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Infra.Cryptography;
using MeAgendaAi.Infra.Extension;
using MeAgendaAi.Infra.JWT;
using MeAgendaAi.Infra.MailJet;
using MeAgendaAi.Infra.Notification;
using Microsoft.Extensions.Logging;

namespace MeAgendaAi.Services;

public class UserService : Service<User>, IUserService
{
	private const int PasswordResetTokenExpirationTimeInSeconds = 3600;
	private const string ActionType = "UserService";
	private readonly IDistributedCacheRepository _distributedCacheRepository;
	private readonly IEmailService _emailService;
	private readonly IJsonWebTokenService _jsonWebTokenService;
	private readonly ILogger<UserService> _logger;
	private readonly IMapper _mapper;
	private readonly NotificationContext _notificationContext;
	private readonly IUserRepository _userRepository;

	public UserService(
		IUserRepository userRepository, NotificationContext notificationContext,
		ILogger<UserService> logger, IJsonWebTokenService jsonWebTokenService,
		IMapper mapper, IDistributedCacheRepository distributedCacheRepository,
		IEmailService emailService) : base(userRepository)
	{
		_userRepository = userRepository;
		_notificationContext = notificationContext;
		_logger = logger;
		_jsonWebTokenService = jsonWebTokenService;
		_mapper = mapper;
		_distributedCacheRepository = distributedCacheRepository;
		_emailService = emailService;
	}

	public async Task<bool> HasUser(string email)
	{
		return await GetByEmailAsync(email) != null;
	}

	public async Task<User?> GetByEmailAsync(string email)
	{
		return await _userRepository.GetEmailAsync(email);
	}

	public bool SamePassword(string password, string confirmPassword)
	{
		return password.Equals(confirmPassword);
	}

	public bool NotSamePassword(string password, string confirmPassword)
	{
		return !SamePassword(password, confirmPassword);
	}

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
			.SetAsync(tokenJwt.RefreshToken.Token, user.Id.ToString(), tokenJwt.RefreshToken.ExpiresIn);

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
			_logger.LogError("[{ActionType}/AuthenticateByRefreshTokenAsync] User {UserIdCached} not found", ActionType,
				userIdCached);
			_notificationContext.AddNotification("User", "User not found");
			return null;
		}

		var tokenJwt = _jsonWebTokenService.GenerateToken(user);

		var response = _mapper.Map<AuthenticateResponse>(user);
		response.IncludeTokenAndRefreshToken(tokenJwt.Token, tokenJwt.RefreshToken.Token);

		await _distributedCacheRepository.RemoveAsync(refreshToken);
		await _distributedCacheRepository
			.SetAsync(tokenJwt.RefreshToken.Token, user.Id.ToString(), tokenJwt.RefreshToken.ExpiresIn);

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
			.SetAsync(identificationToken, user.Id.ToString(), PasswordResetTokenExpirationTimeInSeconds);

		var sended = await _emailService.SendPasswordRecoveryEmail(
			user.Name.FullName,
			user.Email.Address,
			identificationToken,
			PasswordResetTokenExpirationTimeInSeconds
		);

		var notSended = !sended;
		if (notSended)
		{
			_logger.LogError("[{ActionType}/RetrievePasswordAsync] Email not sent to {Email}", ActionType, email);
			_notificationContext.AddNotification("SendEmail", "Email not sent");
			return string.Empty;
		}

		_logger.LogInformation("[{ActionType}/RetrievePasswordAsync] Email successfully sent to {Email}", ActionType,
			email);
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

	public async Task<bool> ResetPassword(string token, string password, string confirmationPassword)
	{
		var userId = await _distributedCacheRepository.GetAsync<Guid>(token);
		if (userId.IsEmpty())
		{
			_logger.LogError("[{ActionType}/ResetPassword] Token not found", ActionType);
			_notificationContext.AddNotification("Token", "Token not found");
			return false;
		}

		var user = await GetByIdAsync(userId);
		if (user is null)
		{
			_logger.LogError("[{ActionType}/ResetPassword] User not found", ActionType);
			_notificationContext.AddNotification("User", "User not found");
			return false;
		}

		if (NotSamePassword(password, confirmationPassword))
		{
			_logger.LogError("[{ActionType}/ResetPassword] The confirmation password is not the same as the password",
				ActionType);
			_notificationContext.AddNotification(
				"ConfirmPassword",
				"The confirmation password is not the same as the password");
			return false;
		}

		user.Encrypt(Encrypt.EncryptString(password, userId.ToString()));
		await _userRepository.UpdateAsync(user);
		await _distributedCacheRepository.RemoveAsync(token);
		return true;
	}
}