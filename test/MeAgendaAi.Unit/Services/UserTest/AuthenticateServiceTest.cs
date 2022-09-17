using System;
using System.Threading.Tasks;
using AutoBogus;
using AutoMapper;
using FluentAssertions;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Infra.JWT;
using MeAgendaAi.Infra.Notification;
using MeAgendaAi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Services.UserTest;

public class AuthenticateServiceTest
{
	private const string ActionType = "UserService";
	private readonly AutoMocker _mocker;
	private readonly UserService _userService;

	public AuthenticateServiceTest()
	{
		_mocker = new AutoMocker();
		_userService = _mocker.CreateInstance<UserService>();
	}

	[SetUp]
	public void SetUp()
	{
		_mocker.GetMock<IUserRepository>().Reset();
		_mocker.GetMock<IMapper>().Reset();
	}

	[Test]
	public async Task AuthenticateAsync_ShouldCallMethodGetByEmailOnce()
	{
		var user = new CompanyBuilder().Generate();
		var request = new AuthenticateRequestBuilder().Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == request.Email)))
			.ReturnsAsync(user);

		_ = await _userService.AuthenticateAsync(request.Email, request.Password);

		_mocker.GetMock<IUserRepository>().Verify(verify => verify.GetEmailAsync(It.IsAny<string>()), Times.Once);
	}

	[Test]
	public async Task AuthenticateAsync_ShouldReturnNullWhenNotFindUser()
	{
		var request = new AuthenticateRequestBuilder().Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == request.Email)));

		var result = await _userService.AuthenticateAsync(request.Email, request.Password);

		result.Should().BeNull();
	}

	[Test]
	public async Task AuthenticateAsync_ShouldAddNotificationWhenNotFindUser()
	{
		var notification = new Notification("User", "User not found!");
		var request = new AuthenticateRequestBuilder().Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == request.Email)));

		_ = await _userService.AuthenticateAsync(request.Email, request.Password);

		_mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
	}

	[Test]
	public async Task AuthenticateAsync_ShouldGenerateAnErrorLogWhenNotFindUser()
	{
		var request = new AuthenticateRequestBuilder().Generate();
		var logMessageExpected = $"[{ActionType}/AuthenticateAsync] User {request.Email} not found";
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == request.Email)));

		_ = await _userService.AuthenticateAsync(request.Email, request.Password);

		_mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public async Task AuthenticateAsync_ShouldReturnNullWhenValidatePasswordAndReturnInvalid()
	{
		var id = Guid.NewGuid();
		var request = new AuthenticateRequestBuilder().Generate();
		var user = new CompanyBuilder()
			.WithId(id)
			.WithPassword("wrong-password")
			.Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == request.Email)))
			.ReturnsAsync(user);

		var response = await _userService.AuthenticateAsync(request.Email, request.Password);

		response.Should().BeNull();
	}

	[Test]
	public async Task AuthenticateAsync_ShouldAddNotificationWhenValidatePasswordAndReturnInvalid()
	{
		var id = Guid.NewGuid();
		var request = new AuthenticateRequestBuilder().Generate();
		var notification = new Notification("User", "Wrong password.");
		var user = new CompanyBuilder()
			.WithId(id)
			.WithPassword("wrong-password")
			.Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == request.Email)))
			.ReturnsAsync(user);

		_ = await _userService.AuthenticateAsync(request.Email, request.Password);

		_mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
	}

	[Test]
	public async Task AuthenticateAsync_ShouldGenerateAnErrorWhenValidatePasswordAndReturnInvalid()
	{
		var id = Guid.NewGuid();
		var request = new AuthenticateRequestBuilder().Generate();
		var user = new CompanyBuilder()
			.WithId(id)
			.WithPassword("wrong-password")
			.Generate();
		var logMessageExpected = $"[{ActionType}/AuthenticateAsync] User {user.Id} Wrong your password";
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == request.Email)))
			.ReturnsAsync(user);

		_ = await _userService.AuthenticateAsync(request.Email, request.Password);

		_mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public async Task AuthenticateAsync_ShouldCallTheMapMethodOnce()
	{
		var id = Guid.NewGuid();
		var request = new AuthenticateRequestBuilder().Generate();
		var password = PasswordBuilder.Encrypt(request.Password, id);
		var user = new CompanyBuilder().WithId(id).WithPassword(password).Generate();
		var tokenExpected = new AutoFaker<JwtToken>().Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == request.Email)))
			.ReturnsAsync(user);
		_mocker.GetMock<IMapper>()
			.Setup(setup => setup.Map<AuthenticateResponse>(It.Is<User>(u => u == user)))
			.Returns(new AuthenticateResponseBuilder().FromUser(user).Generate());
		_mocker.GetMock<IJsonWebTokenService>()
			.Setup(setup => setup.GenerateToken(It.Is<User>(u => u == user)))
			.Returns(tokenExpected);

		_ = await _userService.AuthenticateAsync(request.Email, request.Password);

		_mocker.GetMock<IMapper>()
			.Verify(verify => verify.Map<AuthenticateResponse>(It.Is<User>(u => u == user)), Times.Once());
	}

	[Test]
	public async Task AuthenticateAsync_ShouldCallGenerateTokenpMethodOnce()
	{
		var id = Guid.NewGuid();
		var request = new AuthenticateRequestBuilder().Generate();
		var password = PasswordBuilder.Encrypt(request.Password, id);
		var user = new CompanyBuilder().WithId(id).WithPassword(password).Generate();
		var tokenExpected = new AutoFaker<JwtToken>().Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == request.Email)))
			.ReturnsAsync(user);
		_mocker.GetMock<IMapper>()
			.Setup(setup => setup.Map<AuthenticateResponse>(It.Is<User>(u => u == user)))
			.Returns(new AuthenticateResponseBuilder().FromUser(user).Generate());
		_mocker.GetMock<IJsonWebTokenService>()
			.Setup(setup => setup.GenerateToken(It.Is<User>(u => u == user)))
			.Returns(tokenExpected);

		_ = await _userService.AuthenticateAsync(request.Email, request.Password);

		_mocker.GetMock<IJsonWebTokenService>()
			.Verify(verify => verify.GenerateToken(It.Is<User>(u => u == user)), Times.Once());
	}

	[Test]
	public async Task AuthenticateAsync_ShouldShouldReturnAuthenticateResponseCorrectly()
	{
		var id = Guid.NewGuid();
		var request = new AuthenticateRequestBuilder().Generate();
		var password = PasswordBuilder.Encrypt(request.Password, id);
		var user = new UserBuilder().WithId(id).WithPassword(password).Generate();
		var tokenExpected = new AutoFaker<JwtToken>().Generate();
		var authenticateResponse = new AuthenticateResponseBuilder()
			.FromUser(user)
			.WithToken(tokenExpected.Token)
			.WithRefreshToken(tokenExpected.RefreshToken.Token)
			.Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == request.Email)))
			.ReturnsAsync(user);
		_mocker.GetMock<IMapper>()
			.Setup(setup => setup.Map<AuthenticateResponse>(It.Is<User>(u => u == user)))
			.Returns(new AuthenticateResponseBuilder().FromUser(user).Generate());
		_mocker.GetMock<IJsonWebTokenService>()
			.Setup(setup => setup.GenerateToken(It.Is<User>(u => u == user)))
			.Returns(tokenExpected);

		var response = await _userService.AuthenticateAsync(request.Email, request.Password);

		response.Should().BeEquivalentTo(authenticateResponse);
	}
}