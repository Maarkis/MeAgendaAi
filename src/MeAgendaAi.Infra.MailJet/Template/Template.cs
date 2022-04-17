using MeAgendaAi.Infra.MailJet.Settings;

namespace MeAgendaAi.Infra.MailJet.Template
{
    public abstract class Template
    {
        protected readonly string FromEmail;
        protected readonly string FromName;
        protected readonly string Url;
        private Dictionary<string, int> Templates { get; }

        protected Template(MailSender mailSender)
        {
            Templates = mailSender.Templates;
            FromEmail = mailSender.FromEmail;
            FromName = mailSender.FromName;
            Url = mailSender.PortalUrl;
        }

        protected int GetTemplate(string key) => Templates[key];

        protected static string BuildUrl(string url, string? token = null) =>
            new Uri($"{url}/{token}").ToString();

        protected static int ConvertSecondsInHour(int seconds) => TimeSpan.FromSeconds(seconds).Hours;
    }
}