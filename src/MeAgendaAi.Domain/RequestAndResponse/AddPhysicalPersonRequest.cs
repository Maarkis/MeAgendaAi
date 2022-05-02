using System.Drawing;

namespace MeAgendaAi.Domains.RequestAndResponse;

public class AddPhysicalPersonRequest : AddUserRequest
{
	public AddPhysicalPersonRequest(
		string name, string email, string password,
		string confirmPassword, string surname, string cpf, string rg) : base(name, email, password, confirmPassword)
	{
		Surname = surname;
		CPF = cpf;
		RG = rg;
	}

	public string Surname { get; protected set; }
	public string CPF { get; protected set; }
	public string RG { get; protected set; }
}