namespace MeAgendaAi.Infra.JWT
{
    public class JwtToken
    {
        public string Token { get; set; } = default!;
        public RefreshToken RefreshToken { get; set; } = default!;
    }
}