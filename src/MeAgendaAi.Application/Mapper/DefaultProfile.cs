﻿using AutoMapper;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;

namespace MeAgendaAi.Application.Mapper;

public class DefaultProfile : Profile
{
	public DefaultProfile()
	{
		CreateMap<User, AuthenticateResponse>()
			.ForMember(dest => dest.Id, option => option.MapFrom(source => source.Id))
			.ForMember(dest => dest.Email, option => option.MapFrom(source => source.Email.Address))
			.ForMember(dest => dest.CreatedAt, option => option.MapFrom(source => source.CreatedAt))
			.ForMember(dest => dest.LastUpdatedAt, option => option.MapFrom(source => source.LastUpdatedAt))
			.ForMember(dest => dest.Token, option => option.Ignore())
			.ForMember(dest => dest.RefreshToken, option => option.Ignore());

		CreateMap<PhysicalPerson, PhysicalPersonResponse>()
			.ForMember(dest => dest.Id, option => option.MapFrom(origin => origin.Id))
			.ForMember(dest => dest.Name, option => option.MapFrom(origin => origin.Name.FirstName))
			.ForMember(dest => dest.FullName, option => option.MapFrom(origin => origin.Name.FullName))
			.ForMember(dest => dest.Email, option => option.MapFrom(origin => origin.Email.Address))
			.ForMember(dest => dest.CPF, option => option.MapFrom(origin => origin.CPF));

		CreateMap<AddPhysicalPersonRequest, PhysicalPerson>()
			.ForPath(dest => dest.Email.Address, option => option.MapFrom(origin => origin.Email))
			.ForPath(dest => dest.Name.FirstName, option => option.MapFrom(origin => origin.Name))
			.ForPath(dest => dest.Name.Surname, option => option.MapFrom(origion => origion.Surname))
			.ForPath(dest => dest.Id, option => option.Ignore())
			.ForPath(dest => dest.Valid, option => option.Ignore())
			.ForPath(dest => dest.ValidationResult, option => option.Ignore())
			.ForPath(dest => dest.CreatedAt, option => option.Ignore())
			.ForPath(dest => dest.LastUpdatedAt, option => option.Ignore())
			.ForPath(dest => dest.IsActive, option => option.Ignore())
			.ForPath(dest => dest.PhoneNumbers, option => option.Ignore());
	}
}