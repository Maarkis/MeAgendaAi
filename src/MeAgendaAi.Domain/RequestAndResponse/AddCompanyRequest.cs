namespace MeAgendaAi.Domains.RequestAndResponse
{
    /// <summary>
    /// Request to add a company.
    /// </summary>
    public class AddCompanyRequest : AddUserRequest
    {

        public string CNPJ { get; set; }
        public string Description { get; set; }
        public int LimitCancelHours { get; set; }
        public AddCompanyRequest(
            string name,
            string email,
            string password,
            string confirmPassword,
            string cnpj,
            string description,
            int limitCancelHours) : base(name, email, password, confirmPassword)
        {
            CNPJ = cnpj;
            Description = description;
            LimitCancelHours = limitCancelHours;
        }
    }
}
