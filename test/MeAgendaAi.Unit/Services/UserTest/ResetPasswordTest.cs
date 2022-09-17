using System;
using System.Threading.Tasks;
using FluentAssertions;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Infra.Notification;
using MeAgendaAi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Services.UserTest;

public class ResetPasswordTest
{
	private const string Token = "any-token-reset-password";
	private const string ActionType = "UserService";
	private readonly AutoMocker _mocker;
	private readonly User _user;
	private readonly UserService _userService;

	public ResetPasswordTest()
	{
		_mocker = new AutoMocker();
		_userService = _mocker.CreateInstance<UserService>();
		_user = new UserBuilder().Generate();
	}

	[SetUp]
	public void SetUp()
	{
		_mocker.GetMock<IDistributedCacheRepository>().Reset();
		_mocker.GetMock<ILogger<UserService>>().Reset();
		_mocker.Get<NotificationContext>().Clear();
		_mocker.GetMock<IUserRepository>().Reset();
	}

	[Test]
	public void ResetPassword_ShouldGenerateAnErrorLogWhenNotFindingToken()
	{
		const string token = "unregistered-token";
		var logMessageExpected = $"[{ActionType}/ResetPassword] Token not found";
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == token)));

		_ = _userService.ResetPassword(token, It.IsAny<string>(), It.IsAny<string>());

		_mocker
			.GetMock<ILogger<UserService>>()
			.VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public void ResetPassword_ShouldAddNotificationWhenNotFindingToken()
	{
		const string token = "unregistered-token";
		var notificationExpected = new Notification("Token", "Token not found");
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == token)));

		_ = _userService.ResetPassword(token, It.IsAny<string>(), It.IsAny<string>());

		_mocker
			.Get<NotificationContext>()
			.Notifications
			.Should()
			.ContainEquivalentOf(notificationExpected);
	}

	[Test]
	public async Task ResetPassword_ShouldReturnFalseWhenNotFindingToken()
	{
		const string token = "unregistered-token";
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == token)));

		var result = await _userService.ResetPassword(token, It.IsAny<string>(), It.IsAny<string>());

		result.Should().BeFalse();
	}

	[Test]
	public void ResetPassword_ShouldAddNotificationWhenNotFindingUser()
	{
		var notificationExpected = new Notification("User", "User not found");
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == Token)))
			.ReturnsAsync(_user.Id);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method
				.GetByIdAsync(It.Is<Guid>(prop => prop == _user.Id)))
			.ReturnsAsync((User)null!);

		_ = _userService.ResetPassword(Token, It.IsAny<string>(), It.IsAny<string>());

		_mocker
			.Get<NotificationContext>()
			.Notifications
			.Should()
			.ContainEquivalentOf(notificationExpected);
	}

	[Test]
	public void ResetPassword_ShouldGenerateAnErrorLogWhenNotFindingUser()
	{
		var logMessageExpected = $"[{ActionType}/ResetPassword] User not found";
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == Token)))
			.ReturnsAsync(_user.Id);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method
				.GetByIdAsync(It.Is<Guid>(prop => prop == _user.Id)))
			.ReturnsAsync((User)null!);

		_ = _userService.ResetPassword(Token, It.IsAny<string>(), It.IsAny<string>());

		_mocker
			.GetMock<ILogger<UserService>>()
			.VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public async Task ResetPassword_ShouldReturnFalseWhenNotFindingUser()
	{
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == Token)))
			.ReturnsAsync(_user.Id);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method
				.GetByIdAsync(It.Is<Guid>(prop => prop == _user.Id)))
			.ReturnsAsync((User)null!);

		var result = await _userService.ResetPassword(Token, It.IsAny<string>(), It.IsAny<string>());

		result.Should().BeFalse();
	}

	[Test]
	public void ResetPassword_ShouldAddNotificationWhenPasswordNotSameAsConfirmationPassword()
	{
		const string password = "any-password";
		const string confirmationPasswordIncorrect = "any-other-password";
		var notificationExpected = new Notification(
			"ConfirmPassword",
			"The confirmation password is not the same as the password");
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == Token)))
			.ReturnsAsync(_user.Id);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method
				.GetByIdAsync(It.Is<Guid>(prop => prop == _user.Id)))
			.ReturnsAsync(_user);

		_ = _userService.ResetPassword(Token, password, confirmationPasswordIncorrect);

		_mocker
			.Get<NotificationContext>()
			.Notifications
			.Should()
			.ContainEquivalentOf(notificationExpected);
	}

	[Test]
	public void ResetPassword_ShouldGenerateAnErrorLogWhenWhenPasswordNotSameAsConfirmationPassword()
	{
		const string password = "any-password";
		const string confirmationPasswordIncorrect = "any-other-password";
		var logMessageExpected =
			$"[{ActionType}/ResetPassword] The confirmation password is not the same as the password";
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == Token)))
			.ReturnsAsync(_user.Id);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method
				.GetByIdAsync(It.Is<Guid>(prop => prop == _user.Id)))
			.ReturnsAsync(_user);

		_ = _userService.ResetPassword(Token, password, confirmationPasswordIncorrect);

		_mocker
			.GetMock<ILogger<UserService>>()
			.VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public async Task ResetPassword_ShouldReturnFalseWhenWhenPasswordNotSameAsConfirmationPassword()
	{
		const string password = "any-password";
		const string confirmPassword = "any-other-password";
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == Token)))
			.ReturnsAsync(_user.Id);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method
				.GetByIdAsync(It.Is<Guid>(prop => prop == _user.Id)))
			.ReturnsAsync(_user);

		var result = await _userService.ResetPassword(Token, password, confirmPassword);

		result.Should().BeFalse();
	}

	[Test]
	public void ResetPassword_ShouldCallUpdateAsyncMethodOnce()
	{
		const string password = "any-password";
		const string confirmPassword = "any-password";
		var logMessageExpected =
			$"[{ActionType}/ResetPassword] The confirmation password is not the same as the password";
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == Token)))
			.ReturnsAsync(_user.Id);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method
				.GetByIdAsync(It.Is<Guid>(prop => prop == _user.Id)))
			.ReturnsAsync(_user);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.UpdateAsync(It.Is<User>(prop => prop == _user)))
			.ReturnsAsync(_user);

		_ = _userService.ResetPassword(Token, password, confirmPassword);

		_mocker.GetMock<IUserRepository>()
			.Verify(method => method
					.UpdateAsync(It.Is<User>(propUser => propUser == _user)),
				Times.Once);
	}

	[Test]
	public async Task ResetPassword_ShouldReturnTrueWhenUpdatePassword()
	{
		const string password = "any-password";
		const string confirmPassword = "any-password";
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == Token)))
			.ReturnsAsync(_user.Id);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method
				.GetByIdAsync(It.Is<Guid>(prop => prop == _user.Id)))
			.ReturnsAsync(_user);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.UpdateAsync(It.Is<User>(prop => prop == _user)))
			.ReturnsAsync(_user);

		var result = await _userService.ResetPassword(Token, password, confirmPassword);

		result.Should().BeTrue();
	}


	[Test]
	public void ResetPassword_ShouldCallRemoveAsyncMethodOnce()
	{
		const string password = "any-password";
		const string confirmPassword = "any-password";
		_mocker.GetMock<IDistributedCacheRepository>()
			.Setup(method => method
				.GetAsync<Guid>(It.Is<string>(prop => prop == Token)))
			.ReturnsAsync(_user.Id);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method
				.GetByIdAsync(It.Is<Guid>(prop => prop == _user.Id)))
			.ReturnsAsync(_user);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.UpdateAsync(It.Is<User>(prop => prop == _user)))
			.ReturnsAsync(_user);

		_ = _userService.ResetPassword(Token, password, confirmPassword);

		_mocker.GetMock<IDistributedCacheRepository>()
			.Verify(verify => verify.RemoveAsync(It.Is<string>(prop => prop == Token)),
				Times.Once);
	}
}