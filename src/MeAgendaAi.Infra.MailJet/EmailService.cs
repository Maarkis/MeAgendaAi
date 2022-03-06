using Mailjet.Client;

namespace MeAgendaAi.Infra.MailJet
{
    public interface IEmailService
    {
        Task<bool> SendPasswordRecoveryEmail(string name, string email, string token, int expirationTime);
    }

    public class EmailService : IEmailService
    {
        public IMailjetClient MailjetClient { get; }

        public EmailService(IMailjetClient mailjetClient)
        {
            MailjetClient = mailjetClient;
        }

        public Task<bool> SendPasswordRecoveryEmail(string name, string email, string token, int expirationTime)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> Send(MailjetRequest request)
        {
            var response = await MailjetClient.PostAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}