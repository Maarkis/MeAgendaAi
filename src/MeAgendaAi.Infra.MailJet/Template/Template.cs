using MeAgendaAi.Infra.MailJet.Settings;

namespace MeAgendaAi.Infra.MailJet.Template
{
    public abstract class Template
    {
        protected readonly string FromEmail;
        protected readonly string FromName;
        protected readonly string Url;
        protected Dictionary<string, int> Templates { get; }

        public Template(MailSender mailSender)
        {
            Templates = mailSender.Templates;
            FromEmail = mailSender.FromEmail;
            FromName = mailSender.FromName;
            Url = mailSender.PortalUrl;
        }

        protected int GetTemplate(string key) => Templates[key];

        protected string BuildUrl(string url, string? token = null) =>
            new Uri($"{url}/{token}").ToString();

        protected int ConvertSencondsInHour(int seconds) => TimeSpan.FromSeconds(seconds).Hours;
    }
}