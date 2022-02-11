using FluentValidation.Results;
using System.Net;

namespace MeAgendaAi.Application.Notification
{
    public class NotificationContext
    {
        public string ResponseContentType = "application/json";
        public HttpStatusCode StatusCodes = HttpStatusCode.BadRequest;        

        private readonly List<Notification> _notifications;
        public IReadOnlyCollection<Notification> Notifications => _notifications;
        public bool HasNotifications => _notifications.Any();

        public NotificationContext() => _notifications = new List<Notification>();

        public void AddNotification(string key, string message) => _notifications.Add(new Notification(key, message));
        public void AddNotification(Notification notification) => _notifications.Add(notification);
        public void AddNotifications(IReadOnlyCollection<Notification> notifications) => _notifications.AddRange(notifications);
        public void AddNotifications(IEnumerable<Notification> notifications) => _notifications.AddRange(notifications);
        public void AddNotifications(IList<Notification> notifications) => _notifications.AddRange(notifications);
        public void AddNotifications(ICollection<Notification> notifications) => _notifications.AddRange(notifications);
        public void AddNotifications(params Notification[] notifications) => _notifications.AddRange(notifications);
        public void AddNotifications(ValidationResult validationResult)
        {
            foreach (var err in validationResult.Errors)
                AddNotification(err.PropertyName, err.ErrorMessage);
        }
        public void Clear() => _notifications.Clear();
        public void SetResponseContentType(string contentType) => ResponseContentType = contentType;
        public void SetResponseStatusCode(HttpStatusCode statusCode) => StatusCodes = statusCode;        
    }
}

