using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Integration.SetUp;
using NUnit.Framework;

namespace MeAgendaAi.Integration.Controllers.AuthenticationController;

public class AuthenticateAuthenticationControllerTest : TestBase
{
	protected override string EntryPoint => "Authentication";

	[Test]
	public async Task Authenticate_ShouldReturn200Ok()
	{
		var id = Guid.NewGuid();
		var request = new AuthenticateRequestBuilder().Generate();
		var password = PasswordBuilder.Encrypt(request.Password, id);
		var user = new UserBuilder().WithId(id).WithEmail(request.Email).WithPassword(password).Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "Authenticate"), request);

		response.Should().Be200Ok();
	}

	[Test]
	public async Task Authenticate_ShouldReturnBadRequest400WhenNotFindUser()
	{
		var request = new AuthenticateRequestBuilder().Generate();

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "Authenticate"), request);

		response.Should().Be400BadRequest();
	}

	[Test]
	public async Task Authenticate_ShouldReturnErrorWhenNotFindUser()
	{
		var request = new AuthenticateRequestBuilder().Generate();
		var responseExpected = new ErrorMessage<List<Notification>>(new List<Notification>()
		{
			new Notification("User", "User not found!")
		}, "Errors");

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "Authenticate"), request);

		var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
		content.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public async Task Authenticate_ShouldReturnAnErrorWhenThePasswordIsNotTheSameRegistered()
	{
		var id = Guid.NewGuid();
		var request = new AuthenticateRequestBuilder().Generate();
		var user = new UserBuilder().WithId(id).WithEmail(request.Email).Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		var responseExpected = new ErrorMessage<List<Notification>>(new List<Notification>()
		{
			new Notification("User", "Wrong password.")
		}, "Errors");

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "Authenticate"), request);

		var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
		content.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public async Task Authenticate_ShouldReturnBadRequest400WhenThePasswordIsNotTheSameRegistered()
	{
		var id = Guid.NewGuid();
		var request = new AuthenticateRequestBuilder().Generate();
		var user = new UserBuilder().WithId(id).WithEmail(request.Email).Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "Authenticate"), request);

		response.Should().Be400BadRequest();
	}

	[Test]
	public async Task Authenticate_ShouldReturnAuthenticateResponseCorrectly()
	{
		var id = Guid.NewGuid();
		var request = new AuthenticateRequestBuilder().Generate();
		var password = PasswordBuilder.Encrypt(request.Password, id);
		var user = new UserBuilder().WithId(id).WithEmail(request.Email).WithPassword(password).Generate();
		await DbContext.Users.AddAsync(user);
		await DbContext.SaveChangesAsync();
		var authenticateResponse = new AuthenticateResponseBuilder().FromUser(user).Generate();
		var responseExpected =
			new SuccessMessage<AuthenticateResponse>(authenticateResponse, "Successfully authenticated");

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "Authenticate"), request);

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