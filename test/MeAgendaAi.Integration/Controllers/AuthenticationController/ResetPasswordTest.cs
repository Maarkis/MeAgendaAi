using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Infra.Extension;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Integration.SetUp;
using Microsoft.Extensions.Caching.Distributed;
using NUnit.Framework;

namespace MeAgendaAi.Integration.Controllers.AuthenticationController;

public class ResetPasswordTest : TestBase
{
	private const string EntryPoint = "Authentication";
	
	public ResetPasswordTest()
	{
		
	}

	[Test]
	public async Task ResetPassword_ShouldReturn400BadRequestWhenNotFindingToken()
	{
		var request = new
		{
			Token = "any-token",
			Password = "any-password",
			ConfirmPassword = "any-confirm-password"
		};
		
		var response = await Client.PatchAsJsonAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment("ResetPassword"), request);

		response.Should().Be400BadRequest();
	}
	
	[Test]
	public async Task ResetPassword_ShouldReturnErrorMessageWhenNotFindingToken()
	{
		var request = new
		{
			Token = "any-token",
			Password = "any-password",
			ConfirmPassword = "any-password"
		};
		var notification = new List<Notification>
		{
			new("Token", "Token not found")
		};
		var responseExpected = new ErrorMessage<List<Notification>>(notification, "Errors");
		
		var response = await Client.PatchAsJsonAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment("ResetPassword"), request);
	
		var result = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
		result.Should().BeEquivalentTo(responseExpected);
	}
	
	[Test]
	public async Task ResetPassword_ShouldReturn400BadRequestWhenNotFindingUser()
	{
		const string token = "TokenResetPassword";
		var nonExistingUserId = Guid.NewGuid();
		await DbRedis.SetStringAsync(token, nonExistingUserId.Serialize());
		var request = new
		{
			Token = token,
			Password = "any-password",
			ConfirmPassword = "any-password"
		};
		
		var response = await Client.PatchAsJsonAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment("ResetPassword"), request);

		response.Should().Be400BadRequest();
	}
	
	[Test]
	public async Task ResetPassword_ShouldReturnErrorMessageWhenNotFindingUser()
	{
		const string token = "TokenResetPassword";
		var nonExistingUserId = Guid.NewGuid();
		await DbRedis.SetStringAsync(token, nonExistingUserId.Serialize());
		var notification = new List<Notification>
		{
			new("User", "User not found")
		};
		var responseExpected = new ErrorMessage<List<Notification>>(notification, "Errors");
		var request = new
		{
			Token = token,
			Password = "any-password",
			ConfirmPassword = "any-password"
		};
		
		var response = await Client.PatchAsJsonAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment("ResetPassword"), request);

		var result = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
		result.Should().BeEquivalentTo(responseExpected);
	}
	
	[Test]
	public async Task ResetPassword_ShouldReturn400BadRequestWhenConfirmationPasswordNotSamePassword()
	{
		const string token = "TokenResetPassword";
		var user = new UserBuilder().Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		await DbRedis.SetStringAsync(token, user.Id.Serialize());
		var notification = new List<Notification>
		{
			new("ConfirmPassword", "The confirmation password is not the same as the password")
		};
		var responseExpected = new ErrorMessage<List<Notification>>(notification, "Errors");
		var request = new
		{
			Token = token,
			Password = "any-password",
			ConfirmPassword = "any-incorrect-password-confirmation"
		};
		
		var response = await Client.PatchAsJsonAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment("ResetPassword"), request);

		response.Should().Be400BadRequest();
	}
	
	[Test]
	public async Task ResetPassword_ShouldReturnErrorMessageWhenConfirmationPasswordNotSamePassword()
	{
		const string token = "TokenResetPassword";
		var user = new UserBuilder().Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		await DbRedis.SetStringAsync(token, user.Id.Serialize());
		var notification = new List<Notification>
		{
			new("ConfirmPassword", "The confirmation password is not the same as the password")
		};
		var responseExpected = new ErrorMessage<List<Notification>>(notification, "Errors");
		var request = new
		{
			Token = token,
			Password = "any-password",
			ConfirmPassword = "any-incorrect-password-confirmation"
		};
		
		var response = await Client.PatchAsJsonAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment("ResetPassword"), request);

		var result = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
		result.Should().BeEquivalentTo(responseExpected);
	}
	
	[Test]
	public async Task ResetPassword_ShouldReturn200OkWhenResetPasswordUser()
	{
		const string token = "TokenResetPassword";
		var user = new UserBuilder().Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		await DbRedis.SetStringAsync(token, user.Id.Serialize());
		var messageExpected = new BaseMessage("User password reset successfully", true);
		var request = new
		{
			Token = token,
			Password = "any-password",
			ConfirmPassword = "any-password"
		};
		
		var response = await Client.PatchAsJsonAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment("ResetPassword"), request);

		response.Should().Be200Ok();
	}
	
	[Test]
	public async Task ResetPassword_ShouldReturnSuccessWhenResetPasswordUser()
	{
		const string token = "TokenResetPassword";
		var user = new UserBuilder().Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		await DbRedis.SetStringAsync(token, user.Id.Serialize());
		var messageExpected = new BaseMessage("User password reset successfully", true);
		var request = new
		{
			Token = token,
			Password = "any-password",
			ConfirmPassword = "any-password"
		};
		
		var response = await Client.PatchAsJsonAsync(UrlApi
			.AppendPathSegment(EntryPoint)
			.AppendPathSegment("ResetPassword"), request);

		var result = await response.Content.ReadFromJsonAsync<BaseMessage>();
		result.Should().BeEquivalentTo(messageExpected);
	}
	
}