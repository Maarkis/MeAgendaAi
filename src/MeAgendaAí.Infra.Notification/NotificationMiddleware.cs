using MeAgendaAi.Domains.RequestAndResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace MeAgendaAí.Infra.Notification;

public class NotificationMiddleware : IAsyncResultFilter
{
	private readonly NotificationContext _notificationContext;

	public NotificationMiddleware(NotificationContext notificationContext)
	{
		_notificationContext = notificationContext;
	}

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
		context.HttpContext.Response.StatusCode = (int)_notificationContext.StatusCode;
		context.HttpContext.Response.ContentType = _notificationContext.ResponseContentType;

		var errorMessage =
			new ErrorMessage<IReadOnlyCollection<Notification>>(_notificationContext.Notifications, "Errors");

		var serializedNotifications = JsonConvert.SerializeObject(errorMessage);
		await context.HttpContext.Response.WriteAsync(serializedNotifications);
	}
}