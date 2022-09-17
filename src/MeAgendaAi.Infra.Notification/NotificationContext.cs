using System.Net;
using FluentValidation.Results;

namespace MeAgendaAi.Infra.Notification;

public class NotificationContext
{
	private readonly List<Notification> _notifications;

	public NotificationContext()
	{
		_notifications = new List<Notification>();
	}

	public string ResponseContentType { get; private set; } = "application/json";

	public HttpStatusCode StatusCode { get; private set; } = HttpStatusCode.BadRequest;

	public IReadOnlyCollection<Notification> Notifications => _notifications;
	public bool HasNotifications => _notifications.Any();

	public void AddNotification(string key, string message)
	{
		_notifications.Add(new Notification(key, message));
	}

	public void AddNotification(Notification notification)
	{
		_notifications.Add(notification);
	}

	public void AddNotifications(IEnumerable<Notification> notifications)
	{
		_notifications.AddRange(notifications);
	}

	public void AddNotifications(ValidationResult validationResult)
	{
		foreach (var err in validationResult.Errors)
			AddNotification(err.PropertyName, err.ErrorMessage);
	}

	public void Clear() => _notifications.Clear();

	public void SetResponseContentType(string contentType) => ResponseContentType = contentType;

	public void SetResponseStatusCode(HttpStatusCode statusCode) => StatusCode = statusCode;
}