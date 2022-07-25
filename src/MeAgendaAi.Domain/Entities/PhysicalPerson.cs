using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Entities;

public class PhysicalPerson : User
{
	private PhysicalPerson()
	{
	}

	public PhysicalPerson(string email, string password, string name, string surname, string cpf, string rg) : base(
		email, password, name, surname)
	{
		CPF = cpf;
		RG = rg;

		Validate();
	}

	public string CPF { get; protected set; } = default!;
	public string RG { get; protected set; } = default!;

	public bool Validate()
	{
		return Validate(this, new PhysicalPersonValidator());
	}
}