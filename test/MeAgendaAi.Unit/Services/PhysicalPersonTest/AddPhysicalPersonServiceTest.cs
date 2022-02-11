using FluentAssertions;
using MeAgendaAi.Application.Notification;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services.PhysicalPersonTest
{
    public class AddPhysicalPersonServiceTest
    {
        private Mock<IUserService> _mockUserService;
        private Mock<IPhysicalPersonRepository> _mockPhysicalPersonRepository;
        private Mock<ILogger<PhysicalPersonService>> _mockLogger;
        private NotificationContext _notificationContext;
        private PhysicalPersonService _physicalPersonService;

        private const string ActionType = "PhysicalPersonService";

        public AddPhysicalPersonServiceTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockPhysicalPersonRepository = new Mock<IPhysicalPersonRepository>();
            _mockLogger = new Mock<ILogger<PhysicalPersonService>>();
            _notificationContext = new NotificationContext();
            _physicalPersonService = new PhysicalPersonService(
                _mockUserService.Object,
                _mockPhysicalPersonRepository.Object,
                _notificationContext,
                _mockLogger.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _mockUserService.Reset();
            _mockPhysicalPersonRepository.Reset();
            _mockLogger.Reset();
            _notificationContext.Clear();
        }

        [Test]
        public void AddPhysicalPerson_ShouldInvokeTheHasUserMethodOnce()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);

            _ = _physicalPersonService.AddAsync(request);

            _mockUserService.Verify(verify => verify.HasUser(It.Is<string>(prop => prop == request.Email)), Times.Once());
        }

        [Test]
        public void AddPhysicalPerson_ShouldAddNotificationWhenHasUserReturnTrue()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();
            var physicalPerson = new PhysicalPersonBuilder().ByRequest(request).Generate();
            var notification = new Notification("Email", "Email já cadastrado");
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(true);

            _ = _physicalPersonService.AddAsync(request);

            _notificationContext.Notifications.Should().ContainEquivalentOf(notification);
        }

        [Test]
        public void AddPhysicalPerson_ShouldNotInvokeAddAsyncMethodWhenHasUserReturnTrue()
        {
            var request = new AddPhysicalPersonRequestBuilder().WithNameInvalid().Generate();
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(true);

            _ = _physicalPersonService.AddAsync(request);

            _mockUserService.Verify(verify => verify.AddAsync(It.IsAny<PhysicalPerson>()), Times.Never());
        }

        [Test]
        public void AddPhysicalPerson_ShouldNotInvokeAddAsyncMethodWhenAnEntityIsInvalid()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithNameInvalid().WithSurnameInvalid().Generate();
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == requestInvalid.Email))).ReturnsAsync(false);

            _ = _physicalPersonService.AddAsync(requestInvalid);

            _mockUserService.Verify(verify => verify.AddAsync(It.IsAny<PhysicalPerson>()), Times.Never());
        }

        [Test]
        public void AddPhysicalPerson_ShouldInvokeAddAsyncOfRepositoryMethodWhenAnEntityIsValid()
        {
            var guid = Guid.NewGuid();
            var request = new AddPhysicalPersonRequestBuilder().Generate();
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mockPhysicalPersonRepository
                .Setup(method => method.AddAsync(It.IsAny<PhysicalPerson>()))
                .ReturnsAsync(guid);

            _ = _physicalPersonService.AddAsync(request);

            _mockPhysicalPersonRepository.Verify(verify => verify.AddAsync(It.IsAny<PhysicalPerson>()), Times.Once());
        }

        [Test]
        public async Task AddPhysicalPerson_ShouldInvokeAddAsyncAndReturnId()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();
            var physicalPerson = new PhysicalPersonBuilder().ByRequest(request).Generate();
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mockUserService.Setup(method =>
              method.NotSamePassword(
                  It.Is<string>(password => password == request.Password),
                  It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
              .Returns(false);
            _mockPhysicalPersonRepository
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
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mockUserService.Setup(method =>
                method.NotSamePassword(
                    It.Is<string>(password => password == request.Password),
                    It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword))).Returns(true);

            _ = _physicalPersonService.AddAsync(request);

            _notificationContext.Notifications.Should().ContainEquivalentOf(notification);
        }

        [Test]
        public void AddPhysicalPerson_ShouldNotInvokeAddAsyncMethodWhenNotSamePasswordMethodReturnTrue()
        {
            var request = new AddPhysicalPersonRequestBuilder().WithConfirmPassword("password-different").Generate();
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mockUserService.Setup(method =>
                method.NotSamePassword(
                    It.Is<string>(password => password == request.Password),
                    It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
                .Returns(true);

            _ = _physicalPersonService.AddAsync(request);

            _mockPhysicalPersonRepository.Verify(verify => verify.AddAsync(It.IsAny<PhysicalPerson>()), Times.Never());
        }

        [Test]
        public void AddPhysicalPerson_ShouldGenerateAnErrorLogWhenUserAlreadyExists()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();
            var logMessageExpected = $"[{ActionType}/AddAsync] A registered user for {request.Email} already exists.";
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(true);

            _ = _physicalPersonService.AddAsync(request);

            _mockLogger.VerifyLog(LogLevel.Error, logMessageExpected);
        }

        [Test]
        public void AddPhysicalPerson_ShouldGenerateAnErrorLogWhenNotSamePassword()
        {
            var request = new AddPhysicalPersonRequestBuilder().WithConfirmPassword("password-different").Generate();
            var logMessageExpected = $"[{ActionType}/AddAsync] Confirmation password is not the same as password.";
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mockUserService.Setup(method =>
                method.NotSamePassword(
                    It.Is<string>(password => password == request.Password),
                    It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword))).Returns(true);

            _ = _physicalPersonService.AddAsync(request);

            _mockLogger.VerifyLog(LogLevel.Error, logMessageExpected);
        }

        [Test]
        public void AddPhysicalPerson_ShouldGenerateAnErrorLogWhenCompanyEntityIsInvalid()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().WithNameInvalid().Generate();
            var physicalPersonInvalid = new PhysicalPersonBuilder().ByRequest(requestInvalid).Generate();
            var logMessageExpected = $"[{ActionType}/AddAsync] Invalid information {string.Join(", ", physicalPersonInvalid.ValidationResult.Errors)}";
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == requestInvalid.Email))).ReturnsAsync(false);

            _ = _physicalPersonService.AddAsync(requestInvalid);

            _mockLogger.VerifyLog(LogLevel.Error, logMessageExpected);
        }

        [Test]
        public async Task AddPhysicalPerson_ShouldGenerateAnInformationLogWhenAddCompany()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();
            var physicalPerson = new PhysicalPersonBuilder().ByRequest(request).Generate();
            var logMessageExpected = $"[{ActionType}/AddAsync] User {physicalPerson.Id} registered successfully.";
            _mockUserService.Setup(setup => setup.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mockPhysicalPersonRepository
                    .Setup(method => method.AddAsync(It.IsAny<PhysicalPerson>()))
                    .ReturnsAsync(physicalPerson.Id);

            var response = await _physicalPersonService.AddAsync(request);

            _mockLogger.VerifyLog(LogLevel.Information, logMessageExpected);
        }
    }
}