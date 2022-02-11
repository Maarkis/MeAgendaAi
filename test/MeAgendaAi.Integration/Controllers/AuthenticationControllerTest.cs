using FluentAssertions;
using FluentValidation.Results;
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
    public class AuthenticationControllerTest : TestBase
    {
        [Test]
        public async Task AuthenticationAddPhysicalPerson_ShouldAddAPhysicalPersonTypeUserAndReturn200Ok()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();

            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddPhysicalPerson"), request);

            response.Should().Be200Ok();
        }

        [Test]
        public async Task AuthenticationShouldAddAPhysicalPersonTypeUserAndReturn400BadRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithNameInvalid().Generate();

            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddPhysicalPerson"), requestInvalid);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task AuthenticationAddPhysicalPerson_ShouldAddAPhysicalPersonTypeUserAndReturnBaseResponseWithGuid()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();

            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddPhysicalPerson"), request);
            var conteudo = await response.Content.ReadFromJsonAsync<ResponseBase<Guid>>();

            var physicalPersonInDatabase = await _dbContext.PhysicalPersons.FirstAsync(f => f.Email.Email == request.Email);
            var responseExpected = new ResponseBase<Guid>(physicalPersonInDatabase.Id, "Cadastrado com sucesso", true);
            conteudo.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddPhysicalPerson_ShouldAddAPhysicalPersonTypeUserAndReturnBaseResponseWithGuidAndReturn400BadRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithNameInvalid().Generate();
            var physicalPersonInvalid = new PhysicalPersonBuilder().ByRequest(requestInvalid).Generate();
            var noticationContext = new NotificationContext();
            noticationContext.AddNotifications(physicalPersonInvalid.ValidationResult);
            var listErrorsExpected = new List<Notification>();            
            listErrorsExpected.AddRange(noticationContext.Notifications.ToList());


            var response = await _client.PostAsJsonAsync(AssembleRequisitionTo("Authentication", "AddPhysicalPerson"), requestInvalid);
            var conteudo = await response.Content.ReadFromJsonAsync<ResponseBase<List<Notification>>>();

            var responseExpected = new ResponseBase<List<Notification>>(listErrorsExpected, "Errors", false);
            conteudo.Should().BeEquivalentTo(responseExpected);
        }
    }
}
