namespace MeAgendaAi.Domains.RequestAndResponse
{
    /// <summary>
    /// Request to add a User  
    /// </summary>
    public abstract class AddUserRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }


        public AddUserRequest(string name, string email, string password, string confirmPassword)
        {
            Name = name;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }
    }
}
