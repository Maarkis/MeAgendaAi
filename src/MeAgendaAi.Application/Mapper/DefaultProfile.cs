using AutoMapper;
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
	}
}