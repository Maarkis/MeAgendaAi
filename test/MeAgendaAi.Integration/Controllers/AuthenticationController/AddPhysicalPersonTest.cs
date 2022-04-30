using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Mailjet.Client;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Integration.SetUp;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MeAgendaAi.Integration.Controllers.AuthenticationController;

public class AddPhysicalPersonTest : TestBase
{
	protected override string EntryPoint => "Authentication";

	[Test]
	public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturn201Created()
	{
		Mocker.GetMock<IMailjetClient>()
			.Setup(setup => setup.PostAsync(It.IsAny<MailjetRequest>()))
			.ReturnsAsync(new MailjetResponse(isSuccessStatusCode: true, statusCode: (int)HttpStatusCode.OK,
				It.IsAny<JObject>()));
		var request = new AddPhysicalPersonRequestBuilder().Generate();

		var response =
			await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddPhysicalPerson"), request);

		response.Should().Be201Created();
	}

	[Test]
	public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturn400BadRequest()
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder().WithNameInvalid().Generate();

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddPhysicalPerson"),
			requestInvalid);

		response.Should().Be400BadRequest();
	}

	[Test]
	public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturnBaseResponseWithGuid()
	{
		Mocker.GetMock<IMailjetClient>()
			.Setup(setup => setup.PostAsync(It.IsAny<MailjetRequest>()))
			.ReturnsAsync(new MailjetResponse(isSuccessStatusCode: true, statusCode: (int)HttpStatusCode.OK,
				It.IsAny<JObject>()));
		var request = new AddPhysicalPersonRequestBuilder().Generate();

		var response =
			await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddPhysicalPerson"), request);
		var content = await response.Content.ReadFromJsonAsync<SuccessMessage<Guid>>();

		var physicalPersonInDatabase =
			await DbContext.PhysicalPersons.FirstAsync(f => f.Email.Address == request.Email);
		var responseExpected = new SuccessMessage<Guid>(physicalPersonInDatabase.Id, "Cadastrado com sucesso");
		content.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public async Task AuthenticationAddClient_ShouldReturnErrorMessageWhenTryToAddAPhysicalPersonWithAnInvalidRequest()
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().Generate();
		var messageError = "Request: Email: Can't be empty";
		var responseExpected = new ErrorMessage<string>(messageError, "Invalid requisition");

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddPhysicalPerson"),
			requestInvalid);
		var content = await response.Content.ReadFromJsonAsync<ErrorMessage<string>>();

		content.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public async Task AuthenticationAddClient_ShouldReturn400BadRequestWhenTryToAddAPhysicalPersonWithAnInvalidRequest()
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().Generate();

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddPhysicalPerson"),
			requestInvalid);

		response.Should().Be400BadRequest();
	}

	[Test]
	public async Task AuthenticationAddClient_ShouldReturnAnErrorWhenTryingToAddAnIndividualWithAlreadyRegisteredEmail()
	{
		var physicalPerson = new PhysicalPersonBuilder().Generate();
		await DbContext.PhysicalPersons.AddAsync(physicalPerson);
		await DbContext.SaveChangesAsync();
		var request = new AddPhysicalPersonRequestBuilder().WithEmail(physicalPerson.Email.Address).Generate();
		var listErrorsExpected = new List<Notification>
		{
			new("Email", "Email já cadastrado")
		};
		var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

		var response =
			await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddPhysicalPerson"), request);
		var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

		content.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public async Task
		AuthenticationAddClient_ShouldReturnErrorWhenTryToAddAPhysicalPersonWithPasswordAndConfirmPasswordNotEqual()
	{
		var request = new AddPhysicalPersonRequestBuilder().WithConfirmPassword("different-password").Generate();
		var listErrorsExpected = new List<Notification>
		{
			new("ConfirmPassword", "Senha de confirmação não é igual a senha")
		};
		var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

		var response =
			await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddPhysicalPerson"), request);
		var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

		content.Should().BeEquivalentTo(responseExpected);
	}
}