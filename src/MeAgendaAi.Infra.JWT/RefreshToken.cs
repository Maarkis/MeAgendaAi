namespace MeAgendaAi.Infra.JWT
{
    public class RefreshToken
    {
        public string Token { get; set; } = default!;
        public DateTime ExpiresIn { get; set; }
    }
}