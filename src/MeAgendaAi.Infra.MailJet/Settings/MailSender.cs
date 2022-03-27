namespace MeAgendaAi.Infra.MailJet.Settings
{
    public class MailSender
    {
        public const string SectionName = "MailSender";
        public string FromEmail { get; set; } = default!;
        public string FromName { get; set; } = default!;
        public string PortalUrl { get; set; } = default!;
        public Dictionary<string, int> Templates { get; set; } = default!;
    }
}