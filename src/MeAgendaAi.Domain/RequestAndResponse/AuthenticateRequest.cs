namespace MeAgendaAi.Domains.RequestAndResponse;

public class AuthenticateRequest
{
	public string Email { get; set; } = default!;
	public string Password { get; set; } = default!;
}