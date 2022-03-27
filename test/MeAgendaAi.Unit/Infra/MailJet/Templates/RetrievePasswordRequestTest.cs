using AutoBogus;
using Bogus;
using FluentAssertions;
using MeAgendaAi.Infra.MailJet.Settings;
using MeAgendaAi.Infra.MailJet.Template;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeAgendaAi.Unit.Infra.MailJet.Templates
{
    public class RetrievePasswordRequestTest
    {
        private readonly Faker _faker;
        private readonly MailSender _mailSender;

        private readonly RetrievePasswordRequest _retrievePasswordRequest;

        private readonly string _fullName;
        private readonly string _email;
        private readonly string _token;
        private readonly int _expiration;
        private readonly int _templateId;

        private const string Subject = "Link para alteração da sua senha do Me Agenda Aí";

        public RetrievePasswordRequestTest()
        {
            _faker = new Faker("pt_BR");

            _fullName = _faker.Name.FullName();
            _email = _faker.Internet.Email();
            _token = _faker.Lorem.Word();
            _expiration = 3600;
            _templateId = 1;

            var templates = new Dictionary<string, int>
            {
                { "reset-password", _templateId },
                { "user-confirmation", 2 }
            };

            _mailSender = new AutoFaker<MailSender>()
                .RuleFor(rule => rule.Templates, () => templates)
                .RuleFor(rule => rule.PortalUrl, faker => faker.Internet.Url())
                .Generate();

            _retrievePasswordRequest = new RetrievePasswordRequest(_fullName, _email, _token, _expiration, _mailSender);
        }

        [Test]
        public void RetrievePasswordRequest_ShouldCreateRetrievePasswordRequestTemplateWithFromEmailCorrectly()
        {
            var request = _retrievePasswordRequest.Build();

            var fromEmailResponse = request.Body.Value<string>("FromEmail");
            fromEmailResponse.Should().BeEquivalentTo(_mailSender.FromEmail);
        }

        [Test]
        public void RetrievePasswordRequest_ShouldCreateRetrievePasswordRequestTemplateWithFromNameCorrectly()
        {
            var request = _retrievePasswordRequest.Build();

            var fromNameResponse = request.Body.Value<string>("FromName");
            fromNameResponse.Should().BeEquivalentTo(_mailSender.FromName);
        }

        [Test]
        public void RetrievePasswordRequest_ShouldCreateRetrievePasswordRequestTemplateWithRecipientsCorrectly()
        {
            var request = _retrievePasswordRequest.Build();

            var recipients = request.Body.Value<JArray>("Recipients")?.FirstOrDefault();
            var name = recipients?["Name"]?.Value<string>();
            var email = recipients?["Email"]?.Value<string>();
            name.Should().Be(_fullName);
            email.Should().Be(_email);
        }

        [Test]
        public void RetrievePasswordRequest_ShouldCreateRetrievePasswordRequestTemplateWithSubjectCorrectly()
        {
            var request = _retrievePasswordRequest.Build();

            var subject = request.Body.Value<string>("Subject");
            subject.Should().BeEquivalentTo(Subject);
        }

        [Test]
        public void RetrievePasswordRequest_ShouldCreateRetrievePasswordRequestTemplateWithTemplateCorrectly()
        {
            var request = _retrievePasswordRequest.Build();

            var subject = request.Body.Value<int>("Mj-TemplateID");
            subject.Should().Be(_templateId);
        }

        [Test]
        public void RetrievePasswordRequest_ShouldCreateRetrievePasswordRequestTemplateWithTemplateLanguegeCorrectly()
        {
            var request = _retrievePasswordRequest.Build();

            var subject = request.Body.Value<bool>("Mj-TemplateLanguage");
            subject.Should().BeTrue();
        }

        [Test]
        public void RetrievePasswordRequest_ShouldCreateRetrievePasswordRequestTemplateWithVarsCorrectly()
        {
            var expirationExpected = TimeSpan.FromSeconds(_expiration).Hours;

            var request = _retrievePasswordRequest.Build();
            var vars = request.Body.Value<JToken>("Vars");

            var userName = vars?.Value<string>("user_name");
            var expiration = vars?.Value<int>("expiration");
            var link_reset = vars?.Value<string>("link_reset");
            userName?.Should().BeEquivalentTo(_fullName);
            expiration?.Should().Be(expirationExpected);
            link_reset?.Should().BeEquivalentTo($"{_mailSender.PortalUrl}/{_token}");
        }
    }
}