namespace MeAgendaAi.Infra.MailJet.Settings;

public class ConfigurationMailJet
{
	public const string SectionName = "MailJet";
	public string KeyApiPublic { get; set; } = default!;
	public string KeyApiSecret { get; set; } = default!;
	public string Version { get; set; } = default!;
}