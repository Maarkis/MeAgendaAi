using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Integration.SetUp;
using NUnit.Framework;

namespace MeAgendaAi.Integration.Controllers.AuthenticationController;

public class ActivateTest : TestBase
{
	protected override string EntryPoint => "Authentication";

	[Test]
	public async Task Activate_ShouldReturn400BadRequestWhenNotFindingUser()
	{
		var nonExistingUserId = Guid.NewGuid();
		
		var response = await Client.PatchAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment(nonExistingUserId)
			.AppendPathSegment("Activate"), default!);

		response.Should().Be400BadRequest();
	}

	[Test]
	public async Task Activate_ShouldReturnErrorMessageWhenNotFindingUser()
	{
		var nonExistingUserId = Guid.NewGuid();
		var notification = new List<Notification>
		{
			new("User", "User not found")
		};
		var responseExpected = new ErrorMessage<List<Notification>>(notification, "Errors");
		
		var response = await Client.PatchAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment(nonExistingUserId)
			.AppendPathSegment("Activate"), default!);
		
		var result = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
		result.Should().BeEquivalentTo(responseExpected);
	}
	
	[Test]
	public async Task Activate_ShouldReturn400BadRequestWhenUserIsAlreadyActive()
	{
		var user = new UserBuilder().WithActive().Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		
		var response = await Client.PatchAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment(user.Id)
			.AppendPathSegment("Activate"), default!);

		response.Should().Be400BadRequest();
	}
	
	[Test]
	public async Task Activate_ShouldReturnErrorMessageWhenUserIsAlreadyActive()
	{
		var user = new UserBuilder().WithActive().Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		var notification = new List<Notification>
		{
			new("User", "User is already active")
		};
		var responseExpected = new ErrorMessage<List<Notification>>(notification, "Errors");
		
		var response = await Client.PatchAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment(user.Id)
			.AppendPathSegment("Activate"), default!);
		
		var result = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
		result.Should().BeEquivalentTo(responseExpected);
	}
	
	[Test]
	public async Task Activate_ShouldReturn200OkWhenActivatingUser()
	{
		var user = new UserBuilder().WithDeactivate().Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		
		var response = await Client.PatchAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment(user.Id)
			.AppendPathSegment("Activate"), default!);

		response.Should().Be200Ok();
	}

	[Test]
	public async Task Activate_ShouldReturnMessageSuccessWhenActivatingUser()
	{
		var user = new UserBuilder().WithDeactivate().Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		var resultExpected = new BaseMessage("User activated successfully", success: true);
		
		var response = await Client.PatchAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment(user.Id)
			.AppendPathSegment("Activate"), default!);
		
		var result = await response.Content.ReadFromJsonAsync<BaseMessage>();
		result.Should().BeEquivalentTo(resultExpected);
	}

}