using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeAgendaAi.Infra.MailJet.Template
{
    internal class UserConfirmationRequest : Template
    {
        private readonly string ToName;
        private readonly string ToEmail;
        private readonly string Subject;
        private readonly string Url;
        private readonly string ToUserId;

        private const string KeyTemplate = "user-confirmation";

        public UserConfirmationRequest(string toName, string toEmail, string subject, string url, string toUserId)
        {
            ToName = toName;
            ToEmail = toEmail;
            Subject = subject;
            Url = url;
            ToUserId = toUserId;
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
