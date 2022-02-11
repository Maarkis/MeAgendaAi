using FluentAssertions;
using MeAgendaAi.Application.Notification;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAi.Integration.SetUp;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MeAgendaAi.Integration.Controllers
{
    public class AddClientAuthenticationControllerTest : TestBase
    {
        [Test]
        public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturn201Created()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), request);

            response.Should().Be201Created();
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturn400BadRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithNameInvalid().Generate();

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), requestInvalid);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturnBaseResponseWithGuid()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), request);
            var content = await response.Content.ReadFromJsonAsync<SuccessMessage<Guid>>();

            var physicalPersonInDatabase = await _dbContext.PhysicalPersons.FirstAsync(f => f.Email.Email == request.Email);
            var responseExpected = new SuccessMessage<Guid>(physicalPersonInDatabase.Id, "Cadastrado com sucesso");
            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnErrorMessageWhenTryToAddAPhysicalPersonWithAnInvalidRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().Generate();
            var messageError = "Request: Email: Can't be empty";
            var responseExpected = new ErrorMessage<string>(messageError, "Invalid requisition");

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), requestInvalid);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<string>>();

            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturn400BadRequestWhenTryToAddAPhysicalPersonWithAnInvalidRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().Generate();
            var messageError = "Request: Email: Can't be empty";
            var responseExpected = new ErrorMessage<string>(messageError, "Invalid requisition");

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), requestInvalid);
           
            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnAnErrorWhenTryingToAddAnIndividualWithAlreadyRegisteredEmail()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            await _dbContext.PhysicalPersons.AddAsync(physicalPerson);
            await _dbContext.SaveChangesAsync();
            var request = new AddPhysicalPersonRequestBuilder().WithEmail(physicalPerson.Email.Email).Generate();
            var listErrorsExpected = new List<Notification>();
            listErrorsExpected.Add(new("Email", "Email já cadastrado"));
            var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), request);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnErrorWhenTryToAddAPhysicalPersonWithPasswordAndConfirmPasswordNotEqual()
        {
            var request = new AddPhysicalPersonRequestBuilder().WithConfirmPassword("different-password").Generate();
            var listErrorsExpected = new List<Notification>();
            listErrorsExpected.Add(new("ConfirmPassword", "Senha de confirmação não é igual a senha"));
            var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), request);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

            content.Should().BeEquivalentTo(responseExpected);
        }
    }
    public class AddCompanyAuthenticationControllerTest : TestBase
    {
        [Test]
        public async Task AuthenticaitonAddCompany_ShouldAddCompanyTypeUserAndReturn201Created()
        {
            var request = new AddCompanyRequestBuilder().Generate();

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), request);

            response.Should().Be201Created();
        }

        [Test]
        public async Task AuthenticationAddCompany_ShouldAddCompanyTypeUserAndReturn400BadRequest()
        {
            var requestInvalid = new AddCompanyRequestBuilder().WithNameInvalid().Generate();

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), requestInvalid);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task AuthenticationAddCompany_ShouldAddCompanyTypeUserAndReturnBaseResponseWithGuid()
        {
            var request = new AddCompanyRequestBuilder().Generate();

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), request);
            var content = await response.Content.ReadFromJsonAsync<SuccessMessage<Guid>>();

            var companyInDatabase = await _dbContext.Companies.FirstAsync(f => f.Email.Email == request.Email);
            var responseExpected = new SuccessMessage<Guid>(companyInDatabase.Id, "Cadastrado com sucesso");
            content.Should().BeEquivalentTo(responseExpected);
        }


        [Test]
        public async Task AuthenticationAddClient_ShouldReturnErrorMessageWhenTryToAddAPhysicalPersonWithAnInvalidRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().Generate();
            var messageError = "Request: Email: Can't be empty";
            var responseExpected = new ErrorMessage<string>(messageError, "Invalid requisition");

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), requestInvalid);
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

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), requestInvalid);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

            var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");
            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnAnErrorWhenTryToAddACompanyWithAlreadyRegisteredEmail()
        {
            var company = new CompanyBuilder().Generate();
            await _dbContext.Companies.AddAsync(company);
            await _dbContext.SaveChangesAsync();
            var request = new AddCompanyRequestBuilder().WithEmail(company.Email.Email).Generate();
            var listErrorsExpected = new List<Notification>();
            listErrorsExpected.Add(new("Email", "Email já cadastrado"));
            var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), request);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnErrorWhenTryToAddACompanyWithPasswordAndConfirmPasswordNotEqual()
        {
            var request = new AddCompanyRequestBuilder().WithConfirmPassword("different-password").Generate();
            var listErrorsExpected = new List<Notification>();
            listErrorsExpected.Add(new("ConfirmPassword", "Senha de confirmação não é igual a senha"));
            var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), request);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturn400BadRequestWhenTryToAddACompanyWithAnInvalidRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().Generate();
            var messageError = "Request: Email: Can't be empty";
            var responseExpected = new ErrorMessage<string>(messageError, "Invalid requisition");

            var response = await _client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), requestInvalid);

            response.Should().Be400BadRequest();
        }
    }
}
