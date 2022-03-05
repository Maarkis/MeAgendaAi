namespace MeAgendaAi.Domains.RequestAndResponse
{
    /// <summary>
    /// User authentication response.
    /// </summary>
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string Token { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;

        /// <summary>
        /// Include token and refresh token in response.
        /// </summary>
        /// <param name="token">Authentication token.</param>
        /// <param name="refreshToken">Refresh token.</param>
        public void IncludeTokenAndRefreshToken(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}