using System;
using System.Collections.Generic;
using System.Linq;
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

public class AddCompanyAuthenticationControllerTest : TestBase
{
	protected override string EntryPoint => "Authentication";

	[Test]
	public async Task AuthenticaitonAddCompany_ShouldAddCompanyTypeUserAndReturn201Created()
	{
		Mocker.GetMock<IMailjetClient>()
			.Setup(setup => setup.PostAsync(It.IsAny<MailjetRequest>()))
			.ReturnsAsync(new MailjetResponse(isSuccessStatusCode: true, statusCode: (int)HttpStatusCode.OK,
				It.IsAny<JObject>()));
		var request = new AddCompanyRequestBuilder().Generate();

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddCompany"), request);

		response.Should().Be201Created();
	}

	[Test]
	public async Task AuthenticationAddCompany_ShouldAddCompanyTypeUserAndReturn400BadRequest()
	{
		var requestInvalid = new AddCompanyRequestBuilder().WithNameInvalid().Generate();

		var response =
			await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddCompany"), requestInvalid);

		response.Should().Be400BadRequest();
	}

	[Test]
	public async Task AuthenticationAddCompany_ShouldAddCompanyTypeUserAndReturnBaseResponseWithGuid()
	{
		var request = new AddCompanyRequestBuilder().Generate();

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddCompany"), request);
		var content = await response.Content.ReadFromJsonAsync<SuccessMessage<Guid>>();

		var companyInDatabase = await DbContext.Companies.FirstAsync(f => f.Email.Address == request.Email);
		var responseExpected = new SuccessMessage<Guid>(companyInDatabase.Id, "Cadastrado com sucesso");
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
	public async Task AuthenticationAddClient_ShouldReturnErrorWhenTryToAddACompanyWithLongLengthName()
	{
		var requestInvalid = new AddCompanyRequestBuilder().WithNameInvalid(100).Generate();
		var companyInvalid = new CompanyBuilder().ByRequest(requestInvalid).Generate();
		var noticationContext = new NotificationContext();
		noticationContext.AddNotifications(companyInvalid.ValidationResult);
		var listErrorsExpected = new List<Notification>();
		listErrorsExpected.AddRange(noticationContext.Notifications.ToList());

		var response =
			await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddCompany"), requestInvalid);
		var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

		var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");
		content.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public async Task AuthenticationAddClient_ShouldReturnAnErrorWhenTryToAddACompanyWithAlreadyRegisteredEmail()
	{
		var company = new CompanyBuilder().Generate();
		await DbContext.Companies.AddAsync(company);
		await DbContext.SaveChangesAsync();
		var request = new AddCompanyRequestBuilder().WithEmail(company.Email.Address).Generate();
		var listErrorsExpected = new List<Notification>
		{
			new("Email", "E-mail already registered.")
		};
		var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddCompany"), request);
		var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

		content.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public async Task
		AuthenticationAddClient_ShouldReturnErrorWhenTryToAddACompanyWithPasswordAndConfirmPasswordNotEqual()
	{
		var request = new AddCompanyRequestBuilder().WithConfirmPassword("different-password").Generate();
		var listErrorsExpected = new List<Notification>
		{
			new("ConfirmPassword", "Confirmation password is not the same as password.")
		};
		var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddCompany"), request);
		var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

		content.Should().BeEquivalentTo(responseExpected);
	}

	[Test]
	public async Task AuthenticationAddClient_ShouldReturn400BadRequestWhenTryToAddACompanyWithAnInvalidRequest()
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().Generate();

		var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor(EntryPoint, "AddPhysicalPerson"),
			requestInvalid);

		response.Should().Be400BadRequest();
	}
}