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

            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddPhysicalPerson"), request);

            response.Should().Be201Created();
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturn400BadRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithNameInvalid().Generate();

            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddPhysicalPerson"), requestInvalid);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturnBaseResponseWithGuid()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();

            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddPhysicalPerson"), request);
            var content = await response.Content.ReadFromJsonAsync<ResponseBase<Guid>>();

            var physicalPersonInDatabase = await _dbContext.PhysicalPersons.FirstAsync(f => f.Email.Email == request.Email);
            var responseExpected = new ResponseBase<Guid>(physicalPersonInDatabase.Id, "Cadastrado com sucesso", true);
            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturnBaseResponseWithGuidAndReturn400BadRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithNameInvalid().Generate();
            var physicalPersonInvalid = new PhysicalPersonBuilder().ByRequest(requestInvalid).Generate();
            var noticationContext = new NotificationContext();
            noticationContext.AddNotifications(physicalPersonInvalid.ValidationResult);
            var listErrorsExpected = new List<Notification>();
            listErrorsExpected.AddRange(noticationContext.Notifications.ToList());


            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddPhysicalPerson"), requestInvalid);
            var content = await response.Content.ReadFromJsonAsync<ResponseBase<List<Notification>>>();

            var responseExpected = new ResponseBase<List<Notification>>(listErrorsExpected, "Errors", false);
            content.Should().BeEquivalentTo(responseExpected);
        }
    }
    public class AddCompanyAuthenticationControllerTest : TestBase
    {
        [Test]
        public async Task AuthenticaitonAddCompany_ShouldAddCompanyTypeUserAndReturn201Created()
        {
            var request = new AddCompanyRequestBuilder().Generate();

            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddCompany"), request);

            response.Should().Be201Created();
        }

        [Test]
        public async Task AuthenticationAddCompany_ShouldAddCompanyTypeUserAndReturn400BadRequest()
        {
            var requestInvalid = new AddCompanyRequestBuilder().WithNameInvalid().Generate();

            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddCompany"), requestInvalid);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task AuthenticationAddCompany_ShouldAddCompanyTypeUserAndReturnBaseResponseWithGuid()
        {
            var request = new AddCompanyRequestBuilder().Generate();

            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddCompany"), request);
            var content = await response.Content.ReadFromJsonAsync<ResponseBase<Guid>>();

            var companyInDatabase = await _dbContext.Companies.FirstAsync(f => f.Email.Email == request.Email);
            var responseExpected = new ResponseBase<Guid>(companyInDatabase.Id, "Cadastrado com sucesso", true);
            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddCompany_ShouldAddCompanyTypeUserAndReturnBaseResponseWithGuidAndReturn400BadRequest()
        {
            var requestInvalid = new AddCompanyRequestBuilder().WithNameInvalid().Generate();
            var companyInvalid = new CompanyBuilder().ByRequest(requestInvalid).Generate();
            var noticationContext = new NotificationContext();
            noticationContext.AddNotifications(companyInvalid.ValidationResult);
            var listErrorsExpected = new List<Notification>();
            listErrorsExpected.AddRange(noticationContext.Notifications.ToList());


            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddCompany"), requestInvalid);
            var content = await response.Content.ReadFromJsonAsync<ResponseBase<List<Notification>>>();

            var responseExpected = new ResponseBase<List<Notification>>(listErrorsExpected, "Errors", false);
            content.Should().BeEquivalentTo(responseExpected);
        }

    }
}
