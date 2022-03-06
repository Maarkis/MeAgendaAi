using MeAgendaAi.Infra.CrossCutting;
using Microsoft.Extensions.Options;

namespace MeAgendaAi.Infra.MailJet.Template
{
    internal abstract class Template
    {
        private readonly IOptions<MailSender> optionsMailSender = default!;
        protected readonly string FromEmail;
        protected readonly string FromName;
        public Dictionary<string, int> Templates { get; }

        public Template()
        {
            var mailSender = optionsMailSender.Value;
            Templates = mailSender.Templates;
            FromEmail = mailSender.FromEmail;
            FromName = mailSender.FromName;
        }

        protected int GetTemplate(string key) => Templates[key];

        public string BuildUrl(string url, string id, string? token = null) =>
            new Uri($"{url}/{id}/{token}").ToString();
    }
}