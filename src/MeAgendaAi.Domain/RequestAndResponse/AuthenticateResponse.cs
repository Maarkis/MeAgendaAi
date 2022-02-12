namespace MeAgendaAi.Domains.RequestAndResponse
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string Token { get; set; } = default!;
    }
}