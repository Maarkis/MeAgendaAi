using Mailjet.Client;
using MeAgendaAi.Infra.MailJet.Settings;
using MeAgendaAi.Infra.MailJet.Template;
using Microsoft.Extensions.Options;

namespace MeAgendaAi.Infra.MailJet
{
    public interface IEmailService
    {
        Task<bool> SendPasswordRecoveryEmail(string name, string email, string token, int expirationTime);
    }

    public class EmailService : IEmailService
    {
        public IMailjetClient MailjetClient { get; }
        private readonly MailSender MailSender;

        public EmailService(IMailjetClient mailjetClient, IOptions<MailSender> optionsMailSender)
        {
            MailjetClient = mailjetClient;
            MailSender = optionsMailSender.Value;
        }

        public Task<bool> SendPasswordRecoveryEmail(string name, string email, string token, int expirationTime)
        {
            var retrievePasswordRequest = new RetrievePasswordRequest(name, email, token, expirationTime, MailSender);

            var request = retrievePasswordRequest.Build();

            return Send(request);
        }

        private async Task<bool> Send(MailjetRequest request)
        {
            var response = await MailjetClient.PostAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}