using FluentValidation.Results;
using System.Net;

namespace MeAgendaAí.Infra.Notification
{
    public class NotificationContext
    {
        private string _responseContentType = "application/json";
        public string ResponseContentType => _responseContentType;

        private HttpStatusCode _statusCode = HttpStatusCode.BadRequest;
        public HttpStatusCode StatusCode => _statusCode;

        private readonly List<Notification> _notifications;
        public IReadOnlyCollection<Notification> Notifications => _notifications;
        public bool HasNotifications => _notifications.Any();

        public NotificationContext() => _notifications = new List<Notification>();

        public void AddNotification(string key, string message) => _notifications.Add(new Notification(key, message));

        public void AddNotification(Notification notification) => _notifications.Add(notification);

        public void AddNotifications(IEnumerable<Notification> notifications) => _notifications.AddRange(notifications);

        public void AddNotifications(ValidationResult validationResult)
        {
            foreach (var err in validationResult.Errors)
                AddNotification(err.PropertyName, err.ErrorMessage);
        }

        public void Clear() => _notifications.Clear();

        public void SetResponseContentType(string contentType) => _responseContentType = contentType;

        public void SetResponseStatusCode(HttpStatusCode statusCode) => _statusCode = statusCode;
    }
}