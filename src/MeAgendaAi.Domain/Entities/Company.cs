using MeAgendaAi.Domains.Validators;

namespace MeAgendaAi.Domains.Entities;

public class Company : User
{
	protected Company()
	{
	}

	public Company(string email, string password, string name, string cnpj, string description, int limitCancelHours, IEnumerable<PhoneNumber> phones) :
		base(email, password, name, phones)
	{
		CNPJ = cnpj;
		Description = description;
		LimitCancelHours = limitCancelHours;

		Validate();
	}

	public string CNPJ { get; protected set; } = default!;
	public string Description { get; protected set; } = default!;
	public int LimitCancelHours { get; protected set; }

	public bool Validate() => Validate(this, new CompanyValidator());
}