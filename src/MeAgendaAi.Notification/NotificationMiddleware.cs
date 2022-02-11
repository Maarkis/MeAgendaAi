using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net;

namespace MeAgendaAi.Notification
{
    public class NotificationMiddleware : IAsyncResultFilter
    {
        private readonly NotificationContext _notificationContext;
        public NotificationMiddleware(NotificationContext notificationContext) => _notificationContext = notificationContext;

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (_notificationContext.HasNotifications)
            {
                await SerializeNotifications(context);
                return;
            }
            await next();
        }

        private async Task SerializeNotifications(ResultExecutingContext context)
        {
            context.HttpContext.Response.StatusCode = (int)_notificationContext.StatusCodes;
            context.HttpContext.Response.ContentType = _notificationContext.ResponseContentType;

            var serializedNotifications = JsonConvert.SerializeObject(_notificationContext.Notifications);
            await context.HttpContext.Response.WriteAsync(serializedNotifications);
        }
    }
}
