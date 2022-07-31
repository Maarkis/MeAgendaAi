﻿using AutoMapper;
using FluentAssertions;
using MeAgendaAi.Application.Mapper;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Mapping;

public class DefaultProfileTest
{
	private readonly Mapper _mapper;

	public DefaultProfileTest()
	{
		var configuration = new MapperConfiguration(cfg => cfg.AddProfile<DefaultProfile>());
		_mapper = new Mapper(configuration);
	}

	[Test]
	public void Mapper_ShouldHaveValidConfiguration()
	{
		_mapper.ConfigurationProvider.AssertConfigurationIsValid();
	}

	[Test]
	public void Mapper_ShouldMapPhysicalPersonToPhysicalPersonResponse()
	{
		var physicalPerson = new PhysicalPersonBuilder().Generate();
		var responseExpected = new PhysicalPersonResponseBuilder().WithPhysicalPerson(physicalPerson).Generate();

		var response = _mapper.Map<PhysicalPersonResponse>(physicalPerson);

		response.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public void Mapper_ShouldMapUserToAuthenticateResponseCorrectly()
	{
		var user = new UserBuilder().Generate();
		var authenticateResponseExpected = new AuthenticateResponse
		{
			Id = user.Id,
			Email = user.Email.Address,
			CreatedAt = user.CreatedAt,
			LastUpdatedAt = user.LastUpdatedAt
		};

		var response = _mapper.Map<AuthenticateResponse>(user);

		response.Should().BeEquivalentTo(authenticateResponseExpected);
	}
}