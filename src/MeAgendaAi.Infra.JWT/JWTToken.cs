namespace MeAgendaAi.Infra.JWT
{
    public class JWTToken
    {
        public string Token { get; set; } = default!;
        public RefreshToken RefreshToken { get; set; } = default!;
    }
}