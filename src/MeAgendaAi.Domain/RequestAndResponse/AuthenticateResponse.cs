namespace MeAgendaAi.Domains.RequestAndResponse;

public class AuthenticateResponse
{
	public Guid Id { get; set; }
	public string Email { get; set; } = default!;
	public DateTime CreatedAt { get; set; }
	public DateTime? LastUpdatedAt { get; set; }
	public string Token { get; set; } = default!;
	public string RefreshToken { get; set; } = default!;

	public void IncludeTokenAndRefreshToken(string token, string refreshToke)
	{
		Token = token;
		RefreshToken = refreshToke;
	}
}