using MeAgendaAi.Domains.RequestAndResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;


namespace MeAgendaAi.Application.Notification
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

            var responseBase = new ResponseBase<IReadOnlyCollection<Notification>>(_notificationContext.Notifications, "Errors", false);

            var serializedNotifications = JsonConvert.SerializeObject(responseBase);            
            await context.HttpContext.Response.WriteAsync(serializedNotifications);
        }
    }
}
