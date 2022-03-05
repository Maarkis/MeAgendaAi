namespace MeAgendaAi.Domains.RequestAndResponse
{
    /// <summary>
    /// Request authentication for user.
    /// </summary>
    public class AuthenticateRequest
    {
        public string Email { get; private set; }
        public string Password { get; private set; }

        public AuthenticateRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}