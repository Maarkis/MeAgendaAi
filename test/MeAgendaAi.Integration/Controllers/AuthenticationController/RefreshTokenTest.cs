using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Integration.SetUp;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MeAgendaAi.Integration.Controllers.AuthenticationController;

public class RefreshTokenAuthenticationControllerTest : TestBase
{
	protected override string EntryPoint => "Authentication";
		
	[Test]
	public async Task RefreshToken_ShouldReturn200Ok()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var id = Guid.NewGuid();
		var user = new UserBuilder().WithId(id).Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		await DbRedis.SetStringAsync(refreshToken, JsonConvert.SerializeObject(user.Id));

		var response = await Client.PostAsync(RequisitionAssemblyFor(EntryPoint, "RefreshToken",
			new Dictionary<string, string>()
			{
				{
					"RefreshToken",
					refreshToken
				}
			}), default!);

		response.Should().Be200Ok();
	}

	[Test]
	public async Task RefreshToken_ShouldReturnBadRequest400WhenNotFindRefreshToken()
	{
		var refreshToken = Guid.NewGuid().ToString("N");

		var response = await Client.PostAsync(RequisitionAssemblyFor(EntryPoint, "RefreshToken",
			new Dictionary<string, string>()
			{
				{
					"RefreshToken",
					refreshToken
				}
			}), default!);

		response.Should().Be400BadRequest();
	}

	[Test]
	public async Task RefreshToken_ShouldReturnBadRequest400WhenNotFindUser()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var id = Guid.NewGuid();
		var user = new UserBuilder().WithId(id).Generate();
		await DbRedis.SetStringAsync(refreshToken, JsonConvert.SerializeObject(user.Id));

		var response = await Client.PostAsync(RequisitionAssemblyFor(EntryPoint, "RefreshToken",
			new Dictionary<string, string>()
			{
				{
					"RefreshToken",
					refreshToken
				}
			}), default!);

		response.Should().Be400BadRequest();
	}

	[Test]
	public async Task RefreshToken_ShouldReturnErrorWhenNotFindRefreshToken()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var responseExpected = new ErrorMessage<List<Notification>>(new List<Notification>
		{
			new Notification("Refresh Token", "Refresh token found")
		}, "Errors");

		var response = await Client.PostAsync(RequisitionAssemblyFor(EntryPoint, "RefreshToken",
			new Dictionary<string, string>()
			{
				{
					"RefreshToken",
					refreshToken
				}
			}), default!);

		var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
		content.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public async Task RefreshToken_ShouldReturnErrorWhenNotFindUser()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var id = Guid.NewGuid();
		var user = new UserBuilder().WithId(id).Generate();
		await DbRedis.SetStringAsync(refreshToken, JsonConvert.SerializeObject(user.Id));
		var responseExpected = new ErrorMessage<List<Notification>>(new List<Notification>
		{
			new Notification("User", "User not found")
		}, "Errors");

		var response = await Client.PostAsync(RequisitionAssemblyFor(EntryPoint, "RefreshToken",
			new Dictionary<string, string>()
			{
				{
					"RefreshToken",
					refreshToken
				}
			}), default!);

		var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
		content.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public async Task RefreshToken_ShouldReturnAuthenticateResponseCorrectly()
	{
		var refreshToken = Guid.NewGuid().ToString("N");
		var id = Guid.NewGuid();
		var user = new UserBuilder().WithId(id).Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		await DbRedis.SetStringAsync(refreshToken, JsonConvert.SerializeObject(user.Id));
		var authenticateResponse = new AuthenticateResponseBuilder()
			.FromUser(user)
			.Generate();
		var responseExpected =
			new SuccessMessage<AuthenticateResponse>(authenticateResponse, "Successfully authenticated");

		var response = await Client.PostAsync(RequisitionAssemblyFor(EntryPoint, "RefreshToken",
			new Dictionary<string, string>()
			{
				{
					"RefreshToken",
					refreshToken
				}
			}), default!);

		var content = await response.Content.ReadFromJsonAsync<SuccessMessage<AuthenticateResponse>>();
		content?.Should()
			.BeEquivalentTo(responseExpected, option => option
				.Excluding(prop => prop.Result.Token)
				.Excluding(prop => prop.Result.RefreshToken)
				.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
				.WhenTypeIs<DateTime>());
		content?.Result.Token.Should().NotBeNullOrEmpty();
		content?.Result.RefreshToken.Should().NotBeNullOrEmpty();
	}
}