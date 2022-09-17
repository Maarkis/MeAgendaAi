using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Infra.Notification;
using MeAgendaAi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Services.UserTest;

public class ActivateServiceTest
{
	private const string ActionType = "UserService";
	private readonly AutoMocker _mocker;
	private readonly UserService _userService;

	public ActivateServiceTest()
	{
		_mocker = new AutoMocker();
		_userService = _mocker.CreateInstance<UserService>();
	}

	[SetUp]
	public void SetUp()
	{
		_mocker.Get<NotificationContext>().Clear();
		_mocker.GetMock<ILogger<UserService>>().Reset();
		_mocker.GetMock<IUserRepository>().Reset();
	}

	[Test]
	public void Activate_ShouldGenerateAnErrorLogWhenNotFindUser()
	{
		var userId = Guid.NewGuid();
		var logMessageExpected = $"[{ActionType}/Activate] User {userId} not found";
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(prop => prop == userId)));

		_ = _userService.Activate(userId);

		_mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public void Activate_ShouldAddNotificationWhenNotFindUser()
	{
		var userId = Guid.NewGuid();
		var notification = new Notification("User", "User not found");
		var logMessageExpected = $"[{ActionType}/Activate] User {userId} not found";
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(prop => prop == userId)));

		_ = _userService.Activate(userId);

		_mocker.Get<NotificationContext>()
			.Notifications
			.Should()
			.ContainEquivalentOf(notification);
	}

	[Test]
	public void Activate_ShouldGenerateAnErrorLogWhenUserIsActive()
	{
		var userId = Guid.NewGuid();
		var user = new UserBuilder().WithId(userId).WithActive().Generate();
		var logMessageExpected = $"[{ActionType}/Activate] User {userId} is already active";
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(prop => prop == userId)))
			.ReturnsAsync(user);

		_ = _userService.Activate(userId);

		_mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public void Activate_ShouldAddNotificationWhenUserIsActive()
	{
		var userId = Guid.NewGuid();
		var user = new UserBuilder().WithId(userId).WithActive().Generate();
		var notification = new Notification("User", "User is already active");
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(prop => prop == userId)))
			.ReturnsAsync(user);

		_ = _userService.Activate(userId);

		_mocker.Get<NotificationContext>()
			.Notifications
			.Should()
			.ContainEquivalentOf(notification);
	}

	[Test]
	public void Activate_ShouldActiveUser()
	{
		var userId = Guid.NewGuid();
		var user = new UserBuilder()
			.WithId(userId)
			.WithDeactivate()
			.Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(prop => prop == userId)))
			.ReturnsAsync(user);

		_ = _userService.Activate(userId);

		user.IsActive.Should().BeTrue();
		user.LastUpdatedAt.Should().BeCloseTo(DateTime.Now, 1.Seconds());
	}

	[Test]
	public void Activate_ShouldMethodCallUpdateAsyncOnce()
	{
		var userId = Guid.NewGuid();
		var user = new UserBuilder()
			.WithId(userId)
			.WithDeactivate()
			.Generate();
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.GetByIdAsync(It.Is<Guid>(prop => prop == userId)))
			.ReturnsAsync(user);
		_mocker.GetMock<IUserRepository>()
			.Setup(method => method.UpdateAsync(user))
			.ReturnsAsync(user);

		_ = _userService.Activate(userId);

		_mocker.GetMock<IUserRepository>()
			.Verify(method => method.UpdateAsync(user), Times.Once);
	}
}