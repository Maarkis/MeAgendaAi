namespace MeAgendaAi.Infra.JWT
{
    public class TokenConfiguration
    {
        public const string SectionName = "TokenConfiguration";
        public string Audience { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public int Seconds { get; set; }
        public bool ValidateLifeTime { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateIssuer { get; set; }
    }
}