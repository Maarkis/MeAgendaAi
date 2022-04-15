using AutoBogus;
using Bogus;
using Mailjet.Client;
using MeAgendaAi.Infra.MailJet;
using MeAgendaAi.Infra.MailJet.Settings;
using MeAgendaAi.Infra.MailJet.Template;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;

namespace MeAgendaAi.Unit.Infra.MailJet
{
    public class EmailServiceTest
    {
        private readonly Faker _faker;
        private readonly AutoMocker _mocker;
        private readonly EmailService _emailService;
        private readonly MailSender _mailSender;

        public EmailServiceTest()
        {
            _faker = new Faker("pt_BR");
            _mocker = new AutoMocker();
            var templates = new Dictionary<string, int>
            {
                { "reset-password", 1 },
                { "user-confirmation", 2 }
            };
            _mailSender = new AutoFaker<MailSender>("pt_BR")
                .RuleFor(rule => rule.Templates, () => templates)
                .RuleFor(rule => rule.PortalUrl, faker => faker.Internet.Url())
                .Generate();

            _mocker.GetMock<IOptions<MailSender>>()
                .Setup(setup => setup.Value)
                .Returns(_mailSender);
            _emailService = _mocker.CreateInstance<EmailService>();
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<IMailjetClient>().Reset();
        }

        [Test]
        public void SendPasswordRecoveryEmail_ShouldCallMethodPostAsyncOnce()
        {
            var name = _faker.Name.FullName();
            var email = _faker.Internet.Email();
            var token = _faker.Lorem.Word();
            var expirationTime = _faker.Random.Int(min: 1, max: 24);
            _mocker.GetMock<IMailjetClient>()
                .Setup(setup => setup.PostAsync(It.IsAny<MailjetRequest>()))
                .ReturnsAsync(new MailjetResponse(isSuccessStatusCode: true, statusCode: (int)HttpStatusCode.OK, It.IsAny<JObject>()));

            _ = _emailService.SendPasswordRecoveryEmail(name, email, token, expirationTime);

            _mocker.GetMock<IMailjetClient>()
                .Verify(verify => verify.PostAsync(It.IsAny<MailjetRequest>()), Times.Once());
        }

        [Test]
        public void SendPasswordRecoveryEmail_ShouldCallMethodPostAsyncWithDataCorrectly()
        {
            var name = _faker.Name.FullName();
            var email = _faker.Internet.Email();
            var token = _faker.Lorem.Word();
            var expirationTime = _faker.Random.Int(min: 1, max: 24);            
            _mocker.GetMock<IMailjetClient>()
                .Setup(setup => setup.PostAsync(It.IsAny<MailjetRequest>()))
                .ReturnsAsync(new MailjetResponse(isSuccessStatusCode: true, statusCode: (int)HttpStatusCode.OK, It.IsAny<JObject>()));

            _ = _emailService.SendPasswordRecoveryEmail(name, email, token, expirationTime);

            _mocker.GetMock<IMailjetClient>()
                .Verify(verify => verify.PostAsync(It.Is<MailjetRequest>(request => request.Body == request.Body)), Times.Once());
        }
    }
}