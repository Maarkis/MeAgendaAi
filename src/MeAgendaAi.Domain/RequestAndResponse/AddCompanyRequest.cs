namespace MeAgendaAi.Domains.RequestAndResponse;

public class AddCompanyRequest : AddUserRequest
{
	public AddCompanyRequest(
		string name,
		string email,
		string password,
		string confirmPassword,
		string cnpj,
		string description,
		int limitCancelHours,
		IEnumerable<PhoneRequest> phones) : base(name, email, password, confirmPassword, phones)
	{
		CNPJ = cnpj;
		Description = description;
		LimitCancelHours = limitCancelHours;
	}

	public string CNPJ { get; set; }
	public string Description { get; set; }
	public int LimitCancelHours { get; set; }
}