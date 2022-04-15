using FluentAssertions;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services.CompanyTest
{
    public class AddCompanyServiceTest
    {
        private readonly AutoMocker _mocker;
        private readonly CompanyService _companyService;

        private const string ActionType = "CompanyService";

        public AddCompanyServiceTest()
        {
            _mocker = new AutoMocker();
            _companyService = _mocker.CreateInstance<CompanyService>();
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<IUserService>().Reset();
            _mocker.GetMock<ICompanyRepository>().Reset();
            _mocker.GetMock<ILogger<CompanyService>>().Reset();
            _mocker.Get<NotificationContext>().Clear();
        }

        [Test]
        public void AddCompany_ShouldInvokeTheHasUserMethodOnce()
        {
            var request = new AddCompanyRequestBuilder().Generate();
            _mocker.GetMock<IUserService>()
                .Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
                .ReturnsAsync(false);

            _ = _companyService.AddAsync(request);

            _mocker.GetMock<IUserService>()
                .Verify(verify => verify.HasUser(It.Is<string>(prop => prop == request.Email)), Times.Once());
        }

        [Test]
        public void AddCompany_ShouldAddNotificationWhenHasUserReturnTrue()
        {
            var request = new AddCompanyRequestBuilder().Generate();
            var notification = new Notification("Email", "E-mail already registered.");
            _mocker.GetMock<IUserService>()
                .Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
                .ReturnsAsync(true);

            _ = _companyService.AddAsync(request);

            _mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
        }

        [Test]
        public void AddCompany_ShouldGenerateAnErrorLogWhenUserAlreadyExists()
        {
            var request = new AddCompanyRequestBuilder().Generate();
            var logMessageExpected = $"[{ActionType}/AddAsync] A registered user for {request.Email} already exists.";
            _mocker.GetMock<IUserService>()
                .Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
                .ReturnsAsync(true);

            _ = _companyService.AddAsync(request);

            _mocker.GetMock<ILogger<CompanyService>>().VerifyLog(LogLevel.Error, logMessageExpected);
        }

        [Test]
        public void AddCompany_ShouldAddNotificationsWhenCompanyEntityIsInvalid()
        {
            var requestInvalid = new AddCompanyRequestBuilder().WithEmailInvalid().WithNameInvalid().Generate();
            var companyInvalid = new CompanyBuilder().ByRequest(requestInvalid).Generate();
            var context = new NotificationContext();
            context.AddNotifications(companyInvalid.ValidationResult);
            var notificationsExpected = context.Notifications;
            _mocker.GetMock<IUserService>()
                .Setup(method => method.HasUser(It.Is<string>(prop => prop == requestInvalid.Email)))
                .ReturnsAsync(false);

            _ = _companyService.AddAsync(requestInvalid);

            _mocker.Get<NotificationContext>().Notifications.Should().BeEquivalentTo(notificationsExpected);
        }

        [Test]
        public void AddCompany_ShouldGenerateAnErrorLogWhenCompanyEntityIsInvalid()
        {
            var requestInvalid = new AddCompanyRequestBuilder().WithEmailInvalid().WithNameInvalid().Generate();
            var companyInvalid = new CompanyBuilder().ByRequest(requestInvalid).Generate();
            var logMessageExpected = $"[{ActionType}/AddAsync] Invalid information {string.Join(", ", companyInvalid.ValidationResult.Errors)}";
            _mocker.GetMock<IUserService>()
                .Setup(method => method.HasUser(It.Is<string>(prop => prop == requestInvalid.Email)))
                .ReturnsAsync(false);

            _ = _companyService.AddAsync(requestInvalid);

            _mocker.GetMock<ILogger<CompanyService>>().VerifyLog(LogLevel.Error, logMessageExpected);
        }

        [Test]
        public void AddCompany_ShouldInvokeAddAsyncOfRepositoryMethodWhenAnEntityIsValid()
        {
            var request = new AddCompanyRequestBuilder().Generate();
            _mocker.GetMock<IUserService>().Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            var guid = Guid.NewGuid();
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.AddAsync(It.IsAny<Company>()))
                .ReturnsAsync(guid);

            _ = _companyService.AddAsync(request);

            _mocker.GetMock<ICompanyRepository>().Verify(verify => verify.AddAsync(It.IsAny<Company>()), Times.Once());
        }

        [Test]
        public async Task AddCompany_ShouldInvokeAddAsyncAndReturnId()
        {
            var request = new AddCompanyRequestBuilder().Generate();
            var company = new CompanyBuilder().ByRequest(request).Generate();
            _mocker.GetMock<IUserService>().Setup(setup => setup.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mocker.GetMock<ICompanyRepository>()
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
            _mocker.GetMock<IUserService>()
                .Setup(setup => setup.HasUser(It.Is<string>(prop => prop == request.Email)))
                .ReturnsAsync(false);
            _mocker.GetMock<ICompanyRepository>()
                    .Setup(method => method.AddAsync(It.IsAny<Company>()))
                    .ReturnsAsync(company.Id);

            _ = await _companyService.AddAsync(request);

            _mocker.GetMock<ILogger<CompanyService>>().VerifyLog(LogLevel.Information, logMessageExpected);
        }

        [Test]
        public void AddCompany_ShouldNotInvokeAddAsyncOfdRepositoryMethodWhenAnEntityIsInvalid()
        {
            var requestInvalid = new AddCompanyRequestBuilder().WithEmailInvalid().WithNameInvalid().Generate();
            _mocker.GetMock<IUserService>()
                .Setup(method => method.HasUser(It.Is<string>(prop => prop == requestInvalid.Email)))
                .ReturnsAsync(false);
            _mocker.GetMock<ICompanyRepository>()
                .Setup(method => method.AddAsync(It.IsAny<Company>()))
                .ReturnsAsync(Guid.NewGuid());

            _ = _companyService.AddAsync(requestInvalid);

            _mocker.GetMock<ICompanyRepository>().Verify(verify => verify.AddAsync(It.IsAny<Company>()), Times.Never());
        }

        [Test]
        public void AddCompany_ShouldAddNotificationWhenNotSamePasswordReturnTrue()
        {
            var request = new AddCompanyRequestBuilder().WithConfirmPassword("password-different").Generate();
            var notification = new Notification("ConfirmPassword", "Confirmation password is not the same as password.");
            _mocker.GetMock<IUserService>()
                .Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
                .ReturnsAsync(false);
            _mocker.GetMock<IUserService>()
                .Setup(method => method.NotSamePassword(
                    It.Is<string>(password => password == request.Password),
                    It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
                .Returns(true);

            _ = _companyService.AddAsync(request);

            _mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
        }

        [Test]
        public void AddCompany_ShouldGenerateAnErrorLogWhenNotSamePassword()
        {
            var request = new AddCompanyRequestBuilder().WithConfirmPassword("password-different").Generate();
            var logMessageExpected = $"[{ActionType}/AddAsync] Confirmation password is not the same as password.";
            _mocker.GetMock<IUserService>()
                .Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email)))
                .ReturnsAsync(false);
            _mocker.GetMock<IUserService>()
                .Setup(method => method.NotSamePassword(
                    It.Is<string>(password => password == request.Password),
                    It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
                .Returns(true);

            _ = _companyService.AddAsync(request);

            _mocker.GetMock<ILogger<CompanyService>>().VerifyLog(LogLevel.Error, logMessageExpected);
        }

        [Test]
        public void AddCompany_ShouldNotInvokeAddAsyncMethodWhenNotSamePasswordMethodReturnTrue()
        {
            var request = new AddCompanyRequestBuilder().WithConfirmPassword("password-different").Generate();
            _mocker.GetMock<IUserService>().Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(false);
            _mocker.GetMock<IUserService>()
                .Setup(method => method.NotSamePassword(
                    It.Is<string>(password => password == request.Password),
                    It.Is<string>(confirmPassword => confirmPassword == request.ConfirmPassword)))
               .Returns(true);

            _ = _companyService.AddAsync(request);

            _mocker.GetMock<ICompanyRepository>().Verify(verify => verify.AddAsync(It.IsAny<Company>()), Times.Never());
        }
    }
}