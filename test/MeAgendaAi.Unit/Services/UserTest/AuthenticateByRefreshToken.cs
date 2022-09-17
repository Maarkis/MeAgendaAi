using System;
using System.Threading.Tasks;
using AutoBogus;
using AutoMapper;
using FluentAssertions;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Infra.JWT;
using MeAgendaAi.Infra.Notification;
using MeAgendaAi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Services.UserTest;

public class AuthenticateByRefreshToken
{
	private const string ActionType = "UserService";
	private readonly AutoMocker _mocker;
	private readonly UserService _userService;

	public AuthenticateByRefreshToken()
	{
		_mocker = new AutoMocker();
		_userService = _mocker.CreateInstance<UserService>();
	}

	[SetUp]
	public void SetUp()
	{
		_mocker.GetMock<IDistributedCacheRepository>().Reset();
		_mocker.GetMock<ILogger<UserService>>().Reset();
		_mocker.GetMock<IUserRepository>().Reset();
	}

	[Test]
	public async Task AuthenticateByRefreshTokenAsync_ShouldCallGetAsyncMethodOnce()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var userId = Guid.NewGuid();
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)))
			.ReturnsAsync(userId);

		_ = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

		_mocker.GetMock<IDistributedCacheRepository>()
			.Verify(verify => verify.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)), Times.Once());
	}

	[Test]
	public async Task AuthenticateByRefreshTokenAsync_ShouldGenerateAnErrorLogWhenNotFindRefreshToken()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var logMessageExpected = $"[{ActionType}/AuthenticateByRefreshTokenAsync] Refresh token not found";
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)));

		_ = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

		_mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public async Task AuthenticateByRefreshTokenAsync_ShouldAddNotificationWhenNotFindRefreshToken()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var notification = new Notification("Refresh Token", "Refresh token found");
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)));

		_ = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

		_mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
	}

	[Test]
	public async Task AuthenticateByRefreshTokenAsync_ShouldCallGetByIdAsyncMethodOnce()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var userId = Guid.NewGuid();
		var user = new UserBuilder().WithId(userId).Generate();
		var tokenJWT = new AutoFaker<JwtToken>().Generate();
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)))
			.ReturnsAsync(userId);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(key => key == userId)))
			.ReturnsAsync(user);
		_mocker.GetMock<IMapper>()
			.Setup(setup => setup.Map<AuthenticateResponse>(It.Is<User>(u => u == user)))
			.Returns(new AuthenticateResponseBuilder().FromUser(user).Generate());
		_mocker.GetMock<IJsonWebTokenService>()
			.Setup(setup => setup.GenerateToken(It.Is<User>(u => u == user)))
			.Returns(tokenJWT);

		_ = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

		_mocker.GetMock<IUserRepository>()
			.Verify(verify => verify.GetByIdAsync(It.Is<Guid>(key => key == userId)), Times.Once());
	}

	[Test]
	public async Task AuthenticateByRefreshTokenAsync_ShouldGenerateAnErrorLogWhenNotFindUser()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var userId = Guid.NewGuid();
		var user = new UserBuilder().WithId(userId).Generate();
		var tokenJWT = new AutoFaker<JwtToken>().Generate();
		var logMessageExpected = $"[{ActionType}/AuthenticateByRefreshTokenAsync] User {userId} not found";
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)))
			.ReturnsAsync(userId);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(key => key == userId)));
		_mocker.GetMock<IMapper>()
			.Setup(setup => setup.Map<AuthenticateResponse>(It.Is<User>(u => u == user)))
			.Returns(new AuthenticateResponseBuilder().FromUser(user).Generate());
		_mocker.GetMock<IJsonWebTokenService>()
			.Setup(setup => setup.GenerateToken(It.Is<User>(u => u == user)))
			.Returns(tokenJWT);

		_ = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

		_mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public async Task AuthenticateByRefreshTokenAsync_ShouldAddNotificationWhenNotFindUser()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var userId = Guid.NewGuid();
		var user = new UserBuilder().WithId(userId).Generate();
		var tokenJwt = new AutoFaker<JwtToken>().Generate();
		var notification = new Notification("User", "User not found");
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)))
			.ReturnsAsync(userId);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(key => key == userId)));
		_mocker.GetMock<IMapper>()
			.Setup(setup => setup.Map<AuthenticateResponse>(It.Is<User>(u => u == user)))
			.Returns(new AuthenticateResponseBuilder().FromUser(user).Generate());
		_mocker.GetMock<IJsonWebTokenService>()
			.Setup(setup => setup.GenerateToken(It.Is<User>(u => u == user)))
			.Returns(tokenJwt);

		_ = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

		_mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
	}

	[Test]
	public async Task AuthenticateByRefreshTokenAsync_ShouldCallGenerateTokenpMethodOnce()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var userId = Guid.NewGuid();
		var user = new UserBuilder().WithId(userId).Generate();
		var tokenJWT = new AutoFaker<JwtToken>().Generate();
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)))
			.ReturnsAsync(userId);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(key => key == userId)))
			.ReturnsAsync(user);
		_mocker.GetMock<IMapper>()
			.Setup(setup => setup.Map<AuthenticateResponse>(It.Is<User>(u => u == user)))
			.Returns(new AuthenticateResponseBuilder().FromUser(user).Generate());
		_mocker.GetMock<IJsonWebTokenService>()
			.Setup(setup => setup.GenerateToken(It.Is<User>(u => u == user)))
			.Returns(tokenJWT);

		_ = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

		_mocker.GetMock<IJsonWebTokenService>()
			.Verify(verify => verify.GenerateToken(It.Is<User>(u => u == user)), Times.Once());
	}

	[Test]
	public async Task AuthenticateByRefreshTokenAsync_ShouldCallTheMapMethodOnce()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var userId = Guid.NewGuid();
		var user = new UserBuilder().WithId(userId).Generate();
		var tokenJWT = new AutoFaker<JwtToken>().Generate();
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)))
			.ReturnsAsync(userId);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(key => key == userId)))
			.ReturnsAsync(user);
		_mocker.GetMock<IMapper>()
			.Setup(setup => setup.Map<AuthenticateResponse>(It.Is<User>(u => u == user)))
			.Returns(new AuthenticateResponseBuilder().FromUser(user).Generate());
		_mocker.GetMock<IJsonWebTokenService>()
			.Setup(setup => setup.GenerateToken(It.Is<User>(u => u == user)))
			.Returns(tokenJWT);

		_ = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

		_mocker.GetMock<IMapper>()
			.Verify(verify => verify.Map<AuthenticateResponse>(It.Is<User>(u => u == user)), Times.Once());
	}

	[Test]
	public async Task AuthenticateByRefreshTokenAsync_ShouldCallRemoveAsyncMethodOnce()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var userId = Guid.NewGuid();
		var user = new UserBuilder().WithId(userId).Generate();
		var tokenJWT = new AutoFaker<JwtToken>().Generate();
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)))
			.ReturnsAsync(userId);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(key => key == userId)))
			.ReturnsAsync(user);
		_mocker.GetMock<IMapper>()
			.Setup(setup => setup.Map<AuthenticateResponse>(It.Is<User>(u => u == user)))
			.Returns(new AuthenticateResponseBuilder().FromUser(user).Generate());
		_mocker.GetMock<IJsonWebTokenService>()
			.Setup(setup => setup.GenerateToken(It.Is<User>(u => u == user)))
			.Returns(tokenJWT);

		_ = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

		_mocker.GetMock<IDistributedCacheRepository>()
			.Verify(verify => verify.RemoveAsync(It.Is<string>(key => key == refreshToken)), Times.Once());
	}

	[Test]
	public async Task AuthenticateByRefreshTokenAsync_ShouldCallSetAsyncMethodOnce()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var userId = Guid.NewGuid();
		var user = new UserBuilder().WithId(userId).Generate();
		var tokenJWT = new AutoFaker<JwtToken>().Generate();
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)))
			.ReturnsAsync(userId);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(key => key == userId)))
			.ReturnsAsync(user);
		_mocker.GetMock<IMapper>()
			.Setup(setup => setup.Map<AuthenticateResponse>(It.Is<User>(u => u == user)))
			.Returns(new AuthenticateResponseBuilder().FromUser(user).Generate());
		_mocker.GetMock<IJsonWebTokenService>()
			.Setup(setup => setup.GenerateToken(It.Is<User>(u => u == user)))
			.Returns(tokenJWT);

		_ = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

		_mocker.GetMock<IDistributedCacheRepository>()
			.Verify(verify => verify.SetAsync(
				It.Is<string>(key => key == tokenJWT.RefreshToken.Token),
				It.Is<string>(value => value == userId.ToString()),
				It.Is<DateTime?>(expireIn => expireIn == tokenJWT.RefreshToken.ExpiresIn)), Times.Once());
	}

	[Test]
	public async Task AuthenticateByRefreshTokenAsync_ShouldShouldReturnAuthenticateResponseCorrectly()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var userId = Guid.NewGuid();
		var user = new UserBuilder().WithId(userId).Generate();
		var tokenJWT = new AutoFaker<JwtToken>().Generate();
		var authenticateResponse = new AuthenticateResponseBuilder()
			.FromUser(user)
			.WithToken(tokenJWT.Token)
			.WithRefreshToken(tokenJWT.RefreshToken.Token)
			.Generate();
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method.GetAsync<Guid>(It.Is<string>(key => key == refreshToken)))
			.ReturnsAsync(userId);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(key => key == userId)))
			.ReturnsAsync(user);
		_mocker.GetMock<IMapper>()
			.Setup(setup => setup.Map<AuthenticateResponse>(It.Is<User>(u => u == user)))
			.Returns(new AuthenticateResponseBuilder().FromUser(user).Generate());
		_mocker.GetMock<IJsonWebTokenService>()
			.Setup(setup => setup.GenerateToken(It.Is<User>(u => u == user)))
			.Returns(tokenJWT);

		var response = await _userService.AuthenticateByRefreshTokenAsync(refreshToken);

		response.Should().BeEquivalentTo(authenticateResponse);
	}
}