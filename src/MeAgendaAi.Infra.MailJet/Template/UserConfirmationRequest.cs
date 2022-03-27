using Mailjet.Client;
using Mailjet.Client.Resources;
using MeAgendaAi.Infra.MailJet.Settings;
using Newtonsoft.Json.Linq;

namespace MeAgendaAi.Infra.MailJet.Template
{
    public class UserConfirmationRequest : Template
    {
        private readonly string ToName;
        private readonly string ToEmail;
        private readonly string ToUserId;
        private readonly string Subject;

        private const string KeyTemplate = "user-confirmation";

        public UserConfirmationRequest(string toName, string toEmail, string toUserId, MailSender mailSender) : base(mailSender)
        {
            ToName = toName;
            ToEmail = toEmail;
            ToUserId = toUserId;
            Subject = "Confirmar meu e-mail no Me Agenda Aí";
        }

        public MailjetRequest Build()
        {
            MailjetRequest request = new()
            {
                Resource = Send.Resource
            };

            request.Property(Send.FromEmail, FromEmail);
            request.Property(Send.FromName, FromName);

            request.Property(Send.Recipients,
                new JArray {
                    new JObject {
                        {"Email", ToEmail},
                        {"Name", ToName }
                    }
                });

            request.Property(Send.Subject, Subject);
            request.Property(Send.MjTemplateID, GetTemplate(KeyTemplate));
            request.Property(Send.MjTemplateLanguage, true);

            request.Property(Send.Vars,
                new JObject
                {
                    {"user_name", ToName},
                    {"confirmation_link", BuildUrl(Url, ToUserId)}
                });

            return request;
        }
    }
}