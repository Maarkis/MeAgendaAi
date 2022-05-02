using System;
using System.Threading.Tasks;
using FluentAssertions;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Infra.MailJet;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Services.UserTest;

public class RetrievePasswordTest
{
	private const string ActionType = "UserService";
	private const int ExpirationTimeTokenInSeconds = 3600;
	private readonly AutoMocker _mocker;
	private readonly UserService _userService;

	public RetrievePasswordTest()
	{
		_mocker = new AutoMocker();
		_userService = _mocker.CreateInstance<UserService>();
	}

	[SetUp]
	public void SetUp()
	{
		_mocker.GetMock<IUserRepository>().Reset();
		_mocker.Get<NotificationContext>().Clear();
		_mocker.GetMock<ILogger<UserService>>().Reset();
		_mocker.GetMock<IDistributedCacheRepository>().Reset();
		_mocker.GetMock<IEmailService>().Reset();
	}

	[Test]
	public async Task RetrievePasswordAsync_ShouldCallMethodGetByEmailOnce()
	{
		const string emailRequest = "test-email@hotmail.com";
		var user = new UserBuilder().WithEmail(emailRequest).Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)))
			.ReturnsAsync(user);

		_ = await _userService.RetrievePasswordAsync(emailRequest);

		_mocker.GetMock<IUserRepository>()
			.Verify(verify => verify.GetEmailAsync(It.Is<string>(email => email == emailRequest)), Times.Once);
	}

	[Test]
	public async Task RetrievePasswordAsync_ShouldAddNotificationWhenNotFindUser()
	{
		const string emailRequest = "test-email@hotmail.com";
		var notification = new Notification("User", "User not found");
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)));

		_ = await _userService.RetrievePasswordAsync(emailRequest);

		_mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
	}

	[Test]
	public async Task RetrievePasswordAsync_ShouldGenerateAnErrorLogWhenNotFindUser()
	{
		const string emailRequest = "test-email@hotmail.com";
		var logMessageExpected = $"[{ActionType}/RetrievePasswordAsync] User {emailRequest} not found";
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)));

		_ = await _userService.RetrievePasswordAsync(emailRequest);

		_mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public async Task RetrievePasswordAsync_ShouldCallMethodSetAsyncOnce()
	{
		const string emailRequest = "test-email@hotmail.com";
		var user = new UserBuilder().WithEmail(emailRequest).Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)))
			.ReturnsAsync(user);

		_ = await _userService.RetrievePasswordAsync(emailRequest);

		_mocker.GetMock<IDistributedCacheRepository>()
			.Verify(verify => verify.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()),
				Times.Once());
	}

	[Test]
	public async Task RetrievePasswordAsync_ShouldMethodSetAsyncWithValuesCorrectly()
	{
		var id = Guid.NewGuid();
		const string emailRequest = "test-email@hotmail.com";
		var user = new UserBuilder().WithId(id).WithEmail(emailRequest).Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)))
			.ReturnsAsync(user);

		_ = await _userService.RetrievePasswordAsync(emailRequest);

		_mocker.GetMock<IDistributedCacheRepository>()
			.Verify(verify => verify.SetAsync(
				It.IsAny<string>(),
				It.Is<string>(value => value == id.ToString()),
				It.Is<double>(expireInSeconds => expireInSeconds == ExpirationTimeTokenInSeconds)), Times.Once());
	}

	[Test]
	public async Task RetrievePasswordAsync_ShouldCallMethodSendPasswordRecoveryEmailOnce()
	{
		const string emailRequest = "test-email@hotmail.com";
		var user = new UserBuilder().WithEmail(emailRequest).Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)))
			.ReturnsAsync(user);

		_ = await _userService.RetrievePasswordAsync(emailRequest);

		_mocker.GetMock<IEmailService>()
			.Verify(
				verify => verify.SendPasswordRecoveryEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
					It.IsAny<int>()), Times.Once());
	}

	[Test]
	public async Task RetrievePasswordAsync_ShouldMethodSendPasswordRecoveryEmailWithExactyData()
	{
		const string name = "name-expected";
		const string surname = "surname-expected";
		const string fullNameExpected = $"{name} {surname}";
		const string emailRequest = "test-email@hotmail.com";
		var user = new UserBuilder().WithNameAndSurname(name, surname).WithEmail(emailRequest).Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)))
			.ReturnsAsync(user);

		_ = await _userService.RetrievePasswordAsync(emailRequest);

		_mocker.GetMock<IEmailService>()
			.Verify(verify => verify.SendPasswordRecoveryEmail(
				It.Is<string>(fullName => fullName == fullNameExpected),
				It.Is<string>(email => email == emailRequest),
				It.IsAny<string>(),
				It.Is<int>(expirationInSeconds => expirationInSeconds == ExpirationTimeTokenInSeconds)), Times.Once());
	}

	[Test]
	public async Task RetrievePasswordAsync_ShouldGenerateAnInformationLogWhenTheEmailWasSuccessfullySent()
	{
		const bool sent = true;
		const string emailRequest = "test-email@hotmail.com";
		var logMessageExpected = $"[{ActionType}/RetrievePasswordAsync] Email successfully sent to {emailRequest}";
		var user = new UserBuilder().WithEmail(emailRequest).Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)))
			.ReturnsAsync(user);
		_mocker.GetMock<IEmailService>()
			.Setup(setup =>
				setup.SendPasswordRecoveryEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
					It.IsAny<int>()))
			.ReturnsAsync(sent);

		_ = await _userService.RetrievePasswordAsync(emailRequest);

		_mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Information, logMessageExpected);
	}

	[Test]
	public async Task RetrievePasswordAsync_ShouldAddNotificationWhenTheEmailIsNotSend()
	{
		const bool notSend = false;
		const string emailRequest = "test-email@hotmail.com";
		var user = new UserBuilder().WithEmail(emailRequest).Generate();
		var notificationExpected = new Notification("SendEmail", "Email not sent");
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)))
			.ReturnsAsync(user);
		_mocker.GetMock<IEmailService>()
			.Setup(setup =>
				setup.SendPasswordRecoveryEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
					It.IsAny<int>()))
			.ReturnsAsync(notSend);

		_ = await _userService.RetrievePasswordAsync(emailRequest);

		_mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notificationExpected);
	}

	[Test]
	public async Task RetrievePasswordAsync_ShouldGenerateAnErrorLogWhenTheEmailNotWasSuccessfullySent()
	{
		const bool notSend = false;
		const string emailRequest = "test-email@hotmail.com";
		var user = new UserBuilder().WithEmail(emailRequest).Generate();
		var logMessageExpected = $"[{ActionType}/RetrievePasswordAsync] Email not sent to {emailRequest}";
		_mocker.GetMock<IUserRepository>()
			.Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)))
			.ReturnsAsync(user);
		_mocker.GetMock<IEmailService>()
			.Setup(setup =>
				setup.SendPasswordRecoveryEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
					It.IsAny<int>()))
			.ReturnsAsync(notSend);

		_ = await _userService.RetrievePasswordAsync(emailRequest);

		_mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}
}