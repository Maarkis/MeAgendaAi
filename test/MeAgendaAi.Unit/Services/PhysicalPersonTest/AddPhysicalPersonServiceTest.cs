using System;
using System.Threading.Tasks;
using FluentAssertions;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Infra.MailJet;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Services.PhysicalPersonTest;

public class AddPhysicalPersonServiceTest
{
	private const string ActionType = "PhysicalPersonService";
	private readonly AutoMocker _mocker;
	private readonly PhysicalPersonService _physicalPersonService;

	public AddPhysicalPersonServiceTest()
	{
		_mocker = new AutoMocker();
		_physicalPersonService = _mocker.CreateInstance<PhysicalPersonService>();
	}

	[SetUp]
	public void SetUp()
	{
		_mocker.GetMock<IUserService>().Reset();
		_mocker.GetMock<IPhysicalPersonRepository>().Reset();
		_mocker.GetMock<ILogger<PhysicalPersonService>>().Reset();
		_mocker.Get<NotificationContext>().Clear();
	}

	[Test]
	public void AddPhysicalPerson_ShouldInvokeTheHasUserMethodOnce()
	{
		var request = new AddPhysicalPersonRequestBuilder().Generate();
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(false);

		_ = _physicalPersonService.AddAsync(request);

		_mocker.GetMock<IUserService>()
			.Verify(verify => verify.HasUser(It.Is<string>(prop => prop == request.Email)), Times.Once());
	}

	[Test]
	public void AddPhysicalPerson_ShouldAddNotificationWhenHasUserReturnTrue()
	{
		var request = new AddPhysicalPersonRequestBuilder().Generate();
		var notification = new Notification("Email", "Email já cadastrado");
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(true);

		_ = _physicalPersonService.AddAsync(request);

		_mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
	}

	[Test]
	public void AddPhysicalPerson_ShouldNotInvokeAddAsyncMethodWhenHasUserReturnTrue()
	{
		var request = new AddPhysicalPersonRequestBuilder().WithNameInvalid().Generate();
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(true);

		_ = _physicalPersonService.AddAsync(request);

		_mocker.GetMock<IUserService>()
			.Verify(verify => verify.AddAsync(It.IsAny<PhysicalPerson>()), Times.Never());
	}

	[Test]
	public void AddPhysicalPerson_ShouldNotInvokeAddAsyncMethodWhenAnEntityIsInvalid()
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder().WithNameInvalid().WithSurnameInvalid().Generate();
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == requestInvalid.Email)))
			.ReturnsAsync(false);

		_ = _physicalPersonService.AddAsync(requestInvalid);

		_mocker.GetMock<IUserService>()
			.Verify(verify => verify.AddAsync(It.IsAny<PhysicalPerson>()), Times.Never());
	}

	[Test]
	public void AddPhysicalPerson_ShouldInvokeAddAsyncOfRepositoryMethodWhenAnEntityIsValid()
	{
		var guid = Guid.NewGuid();
		var request = new AddPhysicalPersonRequestBuilder().Generate();
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(false);
		_mocker.GetMock<IPhysicalPersonRepository>()
			.Setup(method => method.AddAsync(It.IsAny<PhysicalPerson>()))
			.ReturnsAsync(guid);

		_ = _physicalPersonService.AddAsync(request);

		_mocker.GetMock<IPhysicalPersonRepository>()
			.Verify(verify => verify.AddAsync(It.IsAny<PhysicalPerson>()), Times.Once());
	}

	[Test]
	public async Task AddPhysicalPerson_ShouldInvokeAddAsyncAndReturnId()
	{
		var request = new AddPhysicalPersonRequestBuilder().Generate();
		var physicalPerson = new PhysicalPersonBuilder().ByRequest(request).Generate();
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(false);
		_mocker.GetMock<IUserService>()
			.Setup(method => method.NotSamePassword(
				It.Is<string>(password => password == request.Password),
				It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
			.Returns(false);
		_mocker.GetMock<IPhysicalPersonRepository>()
			.Setup(method => method.AddAsync(It.IsAny<PhysicalPerson>()))
			.ReturnsAsync(physicalPerson.Id);

		var response = await _physicalPersonService.AddAsync(request);

		response.Should().Be(physicalPerson.Id);
	}

	[Test]
	public void AddPhysicalPerson_ShouldAddNotificationWhenNotSamePasswordReturnTrue()
	{
		var request = new AddPhysicalPersonRequestBuilder().WithConfirmPassword("password-different").Generate();
		var notification = new Notification("ConfirmPassword", "Senha de confirmação não é igual a senha");
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(false);
		_mocker.GetMock<IUserService>()
			.Setup(method => method.NotSamePassword(
				It.Is<string>(password => password == request.Password),
				It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
			.Returns(true);

		_ = _physicalPersonService.AddAsync(request);

		_mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
	}

	[Test]
	public void AddPhysicalPerson_ShouldNotInvokeAddAsyncMethodWhenNotSamePasswordMethodReturnTrue()
	{
		var request = new AddPhysicalPersonRequestBuilder().WithConfirmPassword("password-different").Generate();
		_mocker.GetMock<IUserService>().Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(false);
		_mocker.GetMock<IUserService>()
			.Setup(method => method.NotSamePassword(
				It.Is<string>(password => password == request.Password),
				It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
			.Returns(true);

		_ = _physicalPersonService.AddAsync(request);

		_mocker.GetMock<IPhysicalPersonRepository>()
			.Verify(verify => verify.AddAsync(It.IsAny<PhysicalPerson>()), Times.Never());
	}

	[Test]
	public void AddPhysicalPerson_ShouldGenerateAnErrorLogWhenUserAlreadyExists()
	{
		var request = new AddPhysicalPersonRequestBuilder().Generate();
		var logMessageExpected = $"[{ActionType}/AddAsync] A registered user for {request.Email} already exists";
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(true);

		_ = _physicalPersonService.AddAsync(request);

		_mocker.GetMock<ILogger<PhysicalPersonService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public void AddPhysicalPerson_ShouldGenerateAnErrorLogWhenNotSamePassword()
	{
		var request = new AddPhysicalPersonRequestBuilder().WithConfirmPassword("password-different").Generate();
		var logMessageExpected = $"[{ActionType}/AddAsync] Confirmation password is not the same as password";
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(false);
		_mocker.GetMock<IUserService>()
			.Setup(method => method.NotSamePassword(
				It.Is<string>(password => password == request.Password),
				It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
			.Returns(true);

		_ = _physicalPersonService.AddAsync(request);

		_mocker.GetMock<ILogger<PhysicalPersonService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public void AddPhysicalPerson_ShouldGenerateAnErrorLogWhenCompanyEntityIsInvalid()
	{
		var requestInvalid = new AddPhysicalPersonRequestBuilder()
			.WithEmailInvalid()
			.WithNameInvalid()
			.Generate();
		var physicalPersonInvalid = new PhysicalPersonBuilder()
			.ByRequest(requestInvalid)
			.Generate();
		var logMessageExpected =
			$"[{ActionType}/AddAsync] Invalid information {string.Join(", ", physicalPersonInvalid.ValidationResult.Errors)}";
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == requestInvalid.Email)))
			.ReturnsAsync(false);

		_ = _physicalPersonService.AddAsync(requestInvalid);

		_mocker.GetMock<ILogger<PhysicalPersonService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}

	[Test]
	public async Task AddPhysicalPerson_ShouldGenerateAnInformationLogWhenAddCompany()
	{
		var request = new AddPhysicalPersonRequestBuilder().Generate();
		var physicalPerson = new PhysicalPersonBuilder().ByRequest(request).Generate();
		var logMessageExpected = $"[{ActionType}/AddAsync] User {physicalPerson.Id} registered successfully";
		_mocker.GetMock<IUserService>()
			.Setup(setup => setup.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(false);
		_mocker.GetMock<IPhysicalPersonRepository>()
			.Setup(method => method.AddAsync(It.IsAny<PhysicalPerson>()))
			.ReturnsAsync(physicalPerson.Id);

		_ = await _physicalPersonService.AddAsync(request);

		_mocker.GetMock<ILogger<PhysicalPersonService>>().VerifyLog(LogLevel.Information, logMessageExpected);
	}

	[Test]
	public void AddPhysicalPerson_ShouldAddNotificationWhenWhenNotSendEmailConfirmation()
	{
		var request = new AddPhysicalPersonRequestBuilder().Generate();
		var physicalPerson = new PhysicalPersonBuilder().ByRequest(request).Generate();
		var notification = new Notification("Email", "Confirmation email not sent");
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(false);
		_mocker.GetMock<IUserService>()
			.Setup(method => method.NotSamePassword(
				It.Is<string>(password => password == request.Password),
				It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
			.Returns(false);
		_mocker.GetMock<IEmailService>()
			.Setup(method => method.SendConfirmationEmail(physicalPerson.Name.FullName,
				physicalPerson.Email.Address, physicalPerson.Id.ToString()))
			.ReturnsAsync(false);

		_ = _physicalPersonService.AddAsync(request);


		_mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
	}

	[Test]
	public void AddPhysicalPerson_ShouldGenerateAnErrorLogWhenNotSendEmailConfirmation()
	{
		var request = new AddPhysicalPersonRequestBuilder().Generate();
		var physicalPerson = new PhysicalPersonBuilder().ByRequest(request).Generate();
		var logMessageExpected = $"[{ActionType}/AddAsync] User {physicalPerson.Id} confirmation email not sent";
		_mocker.GetMock<IUserService>()
			.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
			.ReturnsAsync(false);
		_mocker.GetMock<IUserService>()
			.Setup(method => method.NotSamePassword(
				It.Is<string>(password => password == request.Password),
				It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
			.Returns(false);
		_mocker.GetMock<IPhysicalPersonRepository>()
			.Setup(method => method.AddAsync(It.IsAny<PhysicalPerson>()))
			.ReturnsAsync(physicalPerson.Id);
		_mocker.GetMock<IEmailService>()
			.Setup(method => method.SendConfirmationEmail(physicalPerson.Name.FullName,
				physicalPerson.Email.Address, physicalPerson.Id.ToString()))
			.ReturnsAsync(false);

		_ = _physicalPersonService.AddAsync(request);

		_mocker.GetMock<ILogger<PhysicalPersonService>>().VerifyLog(LogLevel.Error, logMessageExpected);
	}
}