namespace MeAgendaAi.Domains.RequestAndResponse
{
    public class AddPhysicalPersonRequest : AddUserRequest
    {
        public string Surname { get; private set; }
        public string CPF { get; private set; }
        public string RG { get; private set; }

        public AddPhysicalPersonRequest(
            string name, string email, string password,
            string confirmPassword, string surname, string cpf, string rg) : base(name, email, password, confirmPassword)
        {
            Surname = surname;
            CPF = cpf;
            RG = rg;
        }
    }
}