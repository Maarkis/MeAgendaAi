namespace MeAgendaAi.Infra.Notification;

public class Notification
{
	public Notification(string key, string message)
	{
		(Key, Message) = (key, message);
	}

	public string Key { get; }
	public string Message { get; }
}