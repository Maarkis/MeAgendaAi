using Mailjet.Client;
using MeAgendaAi.Infra.MailJet.Settings;
using MeAgendaAi.Infra.MailJet.Template;
using Microsoft.Extensions.Options;

namespace MeAgendaAi.Infra.MailJet
{
    public interface IEmailService
    {
        Task<bool> SendPasswordRecoveryEmail(string name, string email, string token, int expirationTime);
        Task<bool> SendConfirmationEmail(string name, string email, string userId);
    }

    public class EmailService : IEmailService
    {
        private IMailjetClient MailjetClient { get; }
        private readonly MailSender _mailSender;

        public EmailService(IMailjetClient mailjetClient, IOptions<MailSender> optionsMailSender)
        {
            MailjetClient = mailjetClient;
            _mailSender = optionsMailSender.Value;
        }

        public Task<bool> SendPasswordRecoveryEmail(string name, string email, string token, int expirationTime)
        {
            var retrievePasswordRequest = new RetrievePasswordRequest(name, email, token, expirationTime, _mailSender);

            var request = retrievePasswordRequest.Build();

            return Send(request);
        }

        public Task<bool> SendConfirmationEmail(string name, string email,  string userId)
        {
            var confirmationEmailRequest = new UserConfirmationRequest(name, email, userId, _mailSender);
            
            var request = confirmationEmailRequest.Build();

            return Send(request);
        }

        private async Task<bool> Send(MailjetRequest request)
        {
            var response = await MailjetClient.PostAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}