﻿using MeAgendaAi.Domains.Validators;

namespace MeAgendaAi.Domains.Entities;

public class Company : User
{
	protected Company()
	{
	}

	public Company(string email, string password, string name, string cnpj, string description, int limitCancelHours) :
		base(email, password, name)
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

	// protected override IEnumerable<object> GetEqualityComponents()
	// {
	// 	yield return base.GetEqualityComponents();
	// 	yield return CNPJ;
	// 	yield return Description;
	// 	yield return LimitCancelHours;
	// }
}