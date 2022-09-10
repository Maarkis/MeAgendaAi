using System;
using System.Collections.Generic;
using System.Linq;
using AutoBogus;
using Bogus;
using FluentAssertions;
using MeAgendaAi.Infra.MailJet.Settings;
using MeAgendaAi.Infra.MailJet.Template;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Infra.MailJet.Templates;

public class UserConfirmationRequestTest
{
	private const string Subject = "Confirmar meu e-mail no Me Agenda Aí";
	private readonly string _email;
	private readonly Faker _faker;

	private readonly string _fullName;
	private readonly string _id;
	private readonly MailSender _mailSender;
	private readonly int _templateId;

	private readonly UserConfirmationRequest _userConfirmationRequestTest;

	public UserConfirmationRequestTest()
	{
		_faker = new Faker("pt_BR");

		_fullName = _faker.Name.FullName();
		_email = _faker.Internet.Email();
		_id = Guid.NewGuid().ToString();
		_templateId = 1;

		var templates = new Dictionary<string, int>
		{
			{ "user-confirmation", _templateId },
			{ "reset-password", 2 }
		};

		_mailSender = new AutoFaker<MailSender>()
			.RuleFor(rule => rule.Templates, () => templates)
			.RuleFor(rule => rule.PortalUrl, faker => faker.Internet.Url())
			.Generate();

		_userConfirmationRequestTest = new UserConfirmationRequest(_fullName, _email, _id, _mailSender);
	}

	[Test]
	public void UserConfirmationRequest_ShouldCreateRetrievePasswordRequestTemplateWithFromEmailCorrectly()
	{
		var request = _userConfirmationRequestTest.Build();

		var fromEmailResponse = request.Body.Value<string>("FromEmail");
		fromEmailResponse.Should().BeEquivalentTo(_mailSender.FromEmail);
	}

	[Test]
	public void UserConfirmationRequest_ShouldCreateRetrievePasswordRequestTemplateWithFromNameCorrectly()
	{
		var request = _userConfirmationRequestTest.Build();

		var fromNameResponse = request.Body.Value<string>("FromName");
		fromNameResponse.Should().BeEquivalentTo(_mailSender.FromName);
	}

	[Test]
	public void UserConfirmationRequest_ShouldCreateRetrievePasswordRequestTemplateWithRecipientsCorrectly()
	{
		var request = _userConfirmationRequestTest.Build();

		var recipients = request.Body.Value<JArray>("Recipients")?.FirstOrDefault();
		var name = recipients?["Name"]?.Value<string>();
		var email = recipients?["Email"]?.Value<string>();
		name.Should().Be(_fullName);
		email.Should().Be(_email);
	}

	[Test]
	public void UserConfirmationRequest_ShouldCreateRetrievePasswordRequestTemplateWithSubjectCorrectly()
	{
		var request = _userConfirmationRequestTest.Build();

		var subject = request.Body.Value<string>("Subject");
		subject.Should().BeEquivalentTo(Subject);
	}

	[Test]
	public void UserConfirmationRequest_ShouldCreateRetrievePasswordRequestTemplateWithTemplateCorrectly()
	{
		var request = _userConfirmationRequestTest.Build();

		var subject = request.Body.Value<int>("Mj-TemplateID");
		subject.Should().Be(_templateId);
	}

	[Test]
	public void UserConfirmationRequest_ShouldCreateRetrievePasswordRequestTemplateWithTemplateLanguegeCorrectly()
	{
		var request = _userConfirmationRequestTest.Build();

		var subject = request.Body.Value<bool>("Mj-TemplateLanguage");
		subject.Should().BeTrue();
	}

	[Test]
	public void UserConfirmationRequest_ShouldCreateRetrievePasswordRequestTemplateWithVarsCorrectly()
	{
		var confirmationLinkExpected = $"{_mailSender.PortalUrl}/{_id}";

		var request = _userConfirmationRequestTest.Build();
		var vars = request.Body.Value<JToken>("Vars");

		var userName = vars?.Value<string>("user_name");
		var confirmationLink = vars?.Value<string>("confirmation_link");
		userName?.Should().BeEquivalentTo(_fullName);
		confirmationLink?.Should().Be(confirmationLinkExpected);
	}
}