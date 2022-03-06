using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace MeAgendaAi.Infra.MailJet.Template
{
    internal class RetrievePasswordRequest : Template
    {
        private readonly string ToName;
        private readonly string ToEmail;
        private readonly string Subject;
        private readonly string Url;
        private readonly string Token;
        private readonly int Expiration;

        private const string KeyTemplate = "reset-password";

        public RetrievePasswordRequest(string toName, string toEmail, string url, string token, int expiration)
        {
            ToName = toName;
            ToEmail = toEmail;           
            Url = url;
            Token = token;
            Expiration = expiration;
            Subject = "Link para alteração da sua senha do Me Agenda Aí";
        }

        public RetrievePasswordRequest(string toName, string toEmail, string subject, string url, string token, int expiration)
        {
            ToName = toName;
            ToEmail = toEmail;
            Subject = subject;
            Url = url;
            Token = token;
            Expiration = expiration;
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
                    {"expiration",  Expiration},
                    {"link_reset", BuildUrl(Url, Token)}
                });

            request.Property(Send.MjTemplateLanguage, true);
            return request;
        }
    }
}