using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using FluentValidation.Results;
using MeAgendaAi.Infra.Notification;
using NUnit.Framework;
using Noti = MeAgendaAi.Infra.Notification.Notification;

namespace MeAgendaAi.Unit.Infra.Notification;

public class NotificationContextTest
{
	private NotificationContext _context;

	public NotificationContextTest()
	{
		_context = new NotificationContext();
	}

	[SetUp]
	public void SetUp()
	{
		_context = new NotificationContext();
	}

	[Test]
	public void NotificationContext_ShouldCreateInstanceWithNotificationsEmpty()
	{
		var context = new NotificationContext();

		context.Notifications.Should().BeEmpty();
	}

	[Test]
	public void NotificationContext_ShouldAddNotificationWithKeyAndMessage()
	{
		var notificationExpected = new Noti("any key", "any message");

		_context.AddNotification("any key", "any message");

		_context.Notifications.Should().ContainEquivalentOf(notificationExpected);
	}

	[Test]
	public void NotificationContext_ShouldAddNotificationWithInstanceNotification()
	{
		var notificationExpected = new Noti("any key", "any message");

		_context.AddNotification(notificationExpected);

		_context.Notifications.Should().ContainEquivalentOf(notificationExpected);
	}

	[Test]
	public void NotificationContext_ShouldAddNotificationsWithListNotification()
	{
		var notifications = new List<Noti>
		{
			new("any key", "any message"),
			new("another key", "another message")
		};

		_context.AddNotifications(notifications);

		_context.Notifications.Should().BeEquivalentTo(notifications);
	}

	[Test]
	public void NotificationContext_ShouldAddNotificationsWithValidationResult()
	{
		var key = "any property name";
		var message = "any message";
		var notificationsExpected = new List<Noti>
		{
			new(key, message)
		};
		var errors = new List<ValidationFailure>
		{
			new(key, message)
		};
		var notifications = new ValidationResult(errors);

		_context.AddNotifications(notifications);

		_context.Notifications.Should().BeEquivalentTo(notificationsExpected);
	}

	[Test]
	public void NotificationContext_ShouldClearNotifications()
	{
		_context.AddNotification("any key", "any message");
		_context.AddNotification("any other key", "any other message");

		_context.Clear();

		_context.Notifications.Should().BeEmpty();
	}

	[Test]
	public void NotificationContext_ShouldHasNotifications()
	{
		_context.AddNotification("any key", "any message");
		_context.AddNotification("any other key", "any other message");

		_context.HasNotifications.Should().BeTrue();
	}

	[Test]
	public void NotificationContext_ShouldNotHaveNotifications()
	{
		_context.HasNotifications.Should().BeFalse();
	}

	[Test]
	public void NotificationContext_ShouldDefineHttpStatusCode()
	{
		_context.SetResponseStatusCode(HttpStatusCode.BadRequest);

		_context.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Test]
	public void NotificationContext_ShouldDefineSetResponseContentType()
	{
		var contentType = "any content type";

		_context.SetResponseContentType(contentType);

		_context.ResponseContentType.Should().Be(contentType);
	}
}