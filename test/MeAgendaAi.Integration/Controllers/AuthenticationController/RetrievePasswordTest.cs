using Bogus;
using FluentAssertions;
using Mailjet.Client;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Integration.SetUp;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MeAgendaAi.Integration.Controllers.AuthenticationController
{
    internal class RetrievePasswordTest : TestBase
    {
        private readonly Faker _faker;
        protected override string EntryPoint => "Authentication";

        public RetrievePasswordTest() =>
            _faker = new Faker("pt_BR");

        [Test]
        public async Task RetrievePassword_ShouldRetrievePasswordReturn200OkWhenSentEmail()
        {
            var email = _faker.Internet.Email();
            var user = new UserBuilder().WithEmail(email).Generate();
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();
            Mocker.GetMock<IMailjetClient>()
               .Setup(setup => setup.PostAsync(It.IsAny<MailjetRequest>()))
               .ReturnsAsync(new MailjetResponse(isSuccessStatusCode: true, statusCode: (int)HttpStatusCode.OK, It.IsAny<JObject>()));

            var response = await Client.PostAsync(RequisitionAssemblyFor(EntryPoint, "RetrievePassword", new Dictionary<string, string>()
            {
                {
                    "Email",
                    email
                }
            }), default!);

            response.Should().Be200Ok();
        }

        [Test]
        public async Task RetrievePassword_ShouldRetrievePasswordReturnSuccessMessageWhenSendingEmail()
        {
            var email = _faker.Internet.Email();
            var user = new UserBuilder().WithEmail(email).Generate();
            var responseExpected = new BaseMessage("Password recovery email sent.", true);
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();
            Mocker.GetMock<IMailjetClient>()
               .Setup(setup => setup.PostAsync(It.IsAny<MailjetRequest>()))
               .ReturnsAsync(new MailjetResponse(isSuccessStatusCode: true, statusCode: (int)HttpStatusCode.OK, It.IsAny<JObject>()));

            var response = await Client.PostAsync(RequisitionAssemblyFor(EntryPoint, "RetrievePassword", new Dictionary<string, string>()
            {
                {
                    "Email",
                    email
                }
            }), default!);

            var result = await response.Content.ReadFromJsonAsync<BaseMessage>();
            result.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task RetrievePassword_ShouldRetrievePasswordReturn400BadRequestWhenNotSentTheEmail()
        {
            var email = _faker.Internet.Email();
            var user = new UserBuilder().WithEmail(email).Generate();
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();
            Mocker.GetMock<IMailjetClient>()
               .Setup(setup => setup.PostAsync(It.IsAny<MailjetRequest>()))
               .ReturnsAsync(new MailjetResponse(isSuccessStatusCode: false, statusCode: (int)HttpStatusCode.BadRequest, It.IsAny<JObject>()));

            var response = await Client.PostAsync(RequisitionAssemblyFor(EntryPoint, "RetrievePassword", new Dictionary<string, string>()
            {
                {
                    "Email",
                    email
                }
            }), default!);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task RetrievePassword_ShouldRetrievePasswordReturnErrorMessageWhenNotSentTheEmail()
        {
            var email = _faker.Internet.Email();
            var user = new UserBuilder().WithEmail(email).Generate();
            var notification = new List<Notification>
            {
                new("SendEmail", "Email not sent" )
            };
            var responseExpected = new ErrorMessage<List<Notification>>(notification, "Errors");
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();
            Mocker.GetMock<IMailjetClient>()
               .Setup(setup => setup.PostAsync(It.IsAny<MailjetRequest>()))
               .ReturnsAsync(new MailjetResponse(isSuccessStatusCode: false, statusCode: (int)HttpStatusCode.BadRequest, It.IsAny<JObject>()));

            var response = await Client.PostAsync(RequisitionAssemblyFor(EntryPoint, "RetrievePassword", new Dictionary<string, string>()
            {
                {
                    "Email",
                    email
                }
            }), default!);

            var result = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
            result.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task RetrievePassword_ShouldRetrievePasswordReturnErrorMessageWhenNotFindUser()
        {
            var email = _faker.Internet.Email();
            var notification = new List<Notification>
            {
                new("User", "User not found")
            };
            var responseExpected = new ErrorMessage<List<Notification>>(notification, "Errors");

            var response = await Client.PostAsync(RequisitionAssemblyFor(EntryPoint, "RetrievePassword", new Dictionary<string, string>()
            {
                {
                    "Email",
                    email
                }
            }), default!);

            var result = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
            result.Should().BeEquivalentTo(responseExpected);
        }
    }
}