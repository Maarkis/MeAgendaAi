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

namespace MeAgendaAi.Unit.Services.CompanyTest
{
    public class AddCompanyServiceTest
    {
        private Mock<IUserService> _mockUserService;
        private Mock<ICompanyRepository> _mockCompanyRepository;
        private Mock<ILogger<CompanyService>> _mockLogger;
        private NotificationContext _notificationContext;
        private Mock<IReport> _report;
        private CompanyService _companyService;

        private const string ActionType = "CompanyService";

        public AddCompanyServiceTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mockLogger = new Mock<ILogger<CompanyService>>();
            _notificationContext = new NotificationContext();
            _report = new Mock<IReport>();
            _companyService = new CompanyService(
                _mockUserService.Object,
                _mockCompanyRepository.Object,
                _notificationContext,
                _report.Object,
                _mockLogger.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _mockUserService.Reset();
            _mockCompanyRepository.Reset();
            _mockLogger.Reset();
            _notificationContext.Clear();
            _report.Reset();
        }

        [Test]
        public void AddCompany_ShouldInvokeTheHasUserMethodOnce()
        {
            var request = new AddCompanyRequestBuilder().Generate();
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);

            _ = _companyService.AddAsync(request);

            _mockUserService.Verify(verify => verify.HasUser(It.Is<string>(prop => prop == request.Email)), Times.Once());
        }

        [Test]
        public void AddCompany_ShouldAddNotificationWhenHasUserReturnTrue()
        {
            var request = new AddCompanyRequestBuilder().Generate();
            var notification = new Notification("Email", "Email já cadastrado");
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(true);

            _ = _companyService.AddAsync(request);

            _notificationContext.Notifications.Should().ContainEquivalentOf(notification);
        }

        [Test]
        public void AddCompany_ShouldGenerateAnErrorLogWhenUserAlreadyExists()
        {
            var request = new AddCompanyRequestBuilder().Generate();
            var logMessageExpected = $"[{ActionType}/AddAsync] A registered user for {request.Email} already exists.";
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(true);

            _ = _companyService.AddAsync(request);

            _mockLogger.VerifyLog(LogLevel.Error, logMessageExpected);
        }

        [Test]
        public void AddCompany_ShouldAddNotificationsWhenCompanyEntityIsInvalid()
        {
            var requestInvalid = new AddCompanyRequestBuilder().WithEmailInvalid().WithNameInvalid().Generate();
            var companyInvalid = new CompanyBuilder().ByRequest(requestInvalid).Generate();
            var context = new NotificationContext();
            context.AddNotifications(companyInvalid.ValidationResult);
            var notificationsExpected = context.Notifications;
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == requestInvalid.Email))).ReturnsAsync(false);

            _ = _companyService.AddAsync(requestInvalid);

            _notificationContext.Notifications.Should().BeEquivalentTo(notificationsExpected);
        }

        [Test]
        public void AddCompany_ShouldGenerateAnErrorLogWhenCompanyEntityIsInvalid()
        {
            var requestInvalid = new AddCompanyRequestBuilder().WithEmailInvalid().WithNameInvalid().Generate();
            var companyInvalid = new CompanyBuilder().ByRequest(requestInvalid).Generate();
            var logMessageExpected = $"[{ActionType}/AddAsync] Invalid information {string.Join(", ", companyInvalid.ValidationResult.Errors)}";
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == requestInvalid.Email))).ReturnsAsync(false);

            _ = _companyService.AddAsync(requestInvalid);

            _mockLogger.VerifyLog(LogLevel.Error, logMessageExpected);
        }

        [Test]
        public void AddCompany_ShouldInvokeAddAsyncOfRepositoryMethodWhenAnEntityIsValid()
        {
            var request = new AddCompanyRequestBuilder().Generate();
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            var guid = Guid.NewGuid();
            _mockCompanyRepository.Setup(method => method.AddAsync(It.IsAny<Company>())).ReturnsAsync(guid);

            _ = _companyService.AddAsync(request);

            _mockCompanyRepository.Verify(verify => verify.AddAsync(It.IsAny<Company>()), Times.Once());
        }

        [Test]
        public async Task AddCompany_ShouldInvokeAddAsyncAndReturnId()
        {
            var request = new AddCompanyRequestBuilder().Generate();
            var company = new CompanyBuilder().ByRequest(request).Generate();
            _mockUserService.Setup(setup => setup.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mockCompanyRepository
                    .Setup(method => method.AddAsync(It.IsAny<Company>()))
                    .ReturnsAsync(company.Id);

            var response = await _companyService.AddAsync(request);

            response.Should().Be(company.Id);
        }

        [Test]
        public async Task AddCompany_ShouldGenerateAnInformationLogWhenAddCompany()
        {
            var request = new AddCompanyRequestBuilder().Generate();
            var company = new CompanyBuilder().ByRequest(request).Generate();
            var logMessageExpected = $"[{ActionType}/AddAsync] User {company.Id} registered successfully.";
            _mockUserService.Setup(setup => setup.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mockCompanyRepository
                    .Setup(method => method.AddAsync(It.IsAny<Company>()))
                    .ReturnsAsync(company.Id);

            var response = await _companyService.AddAsync(request);

            _mockLogger.VerifyLog(LogLevel.Information, logMessageExpected);
        }

        [Test]
        public void AddCompany_ShouldNotInvokeAddAsyncOfdRepositoryMethodWhenAnEntityIsInvalid()
        {
            var requestInvalid = new AddCompanyRequestBuilder().WithEmailInvalid().WithNameInvalid().Generate();
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == requestInvalid.Email))).ReturnsAsync(false);
            _mockCompanyRepository.Setup(method => method.AddAsync(It.IsAny<Company>())).ReturnsAsync(Guid.NewGuid());

            _ = _companyService.AddAsync(requestInvalid);

            _mockCompanyRepository.Verify(verify => verify.AddAsync(It.IsAny<Company>()), Times.Never());
        }

        [Test]
        public void AddCompany_ShouldAddNotificationWhenNotSamePasswordReturnTrue()
        {
            var request = new AddCompanyRequestBuilder().WithConfirmPassword("password-different").Generate();
            var notification = new Notification("ConfirmPassword", "Senha de confirmação não é igual a senha");
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mockUserService.Setup(method =>
                method.NotSamePassword(
                    It.Is<string>(password => password == request.Password),
                    It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword))).Returns(true);

            _ = _companyService.AddAsync(request);

            _notificationContext.Notifications.Should().ContainEquivalentOf(notification);
        }

        [Test]
        public void AddCompany_ShouldGenerateAnErrorLogWhenNotSamePassword()
        {
            var request = new AddCompanyRequestBuilder().WithConfirmPassword("password-different").Generate();
            var logMessageExpected = $"[{ActionType}/AddAsync] Confirmation password is not the same as password.";
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mockUserService.Setup(method =>
                method.NotSamePassword(
                    It.Is<string>(password => password == request.Password),
                    It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword))).Returns(true);

            _ = _companyService.AddAsync(request);

            _mockLogger.VerifyLog(LogLevel.Error, logMessageExpected);
        }

        [Test]
        public void AddCompany_ShouldNotInvokeAddAsyncMethodWhenNotSamePasswordMethodReturnTrue()
        {
            var request = new AddCompanyRequestBuilder().WithConfirmPassword("password-different").Generate();
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mockUserService.Setup(method =>
                method.NotSamePassword(
                    It.Is<string>(password => password == request.Password),
                    It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
                .Returns(true);

            _ = _companyService.AddAsync(request);

            _mockCompanyRepository.Verify(verify => verify.AddAsync(It.IsAny<Company>()), Times.Never());
        }
    }
}