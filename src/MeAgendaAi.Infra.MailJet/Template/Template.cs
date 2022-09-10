using MeAgendaAi.Infra.MailJet.Settings;

namespace MeAgendaAi.Infra.MailJet.Template;

public abstract class Template
{
	protected readonly string FromEmail;
	protected readonly string FromName;
	protected readonly string Url;

	protected Template(MailSender mailSender)
	{
		Templates = mailSender.Templates;
		FromEmail = mailSender.FromEmail;
		FromName = mailSender.FromName;
		Url = mailSender.PortalUrl;
	}

	private Dictionary<string, int> Templates { get; }

	protected int GetTemplate(string key)
	{
		return Templates[key];
	}

	protected static string BuildUrl(string url, string? token = null)
	{
		return new Uri($"{url}/{token}").ToString();
	}

	protected static int ConvertSecondsInHour(int seconds)
	{
		return TimeSpan.FromSeconds(seconds).Hours;
	}
}