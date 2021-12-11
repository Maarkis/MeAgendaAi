using FluentAssertions;
using MeAgendaAi.Application.Notification;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services
{
    public class CompanyServiceTest
    {
        private Mock<IUserService> _mockUserService;
        private Mock<ICompanyRepository> _mockCompanyRepository;
        private NotificationContext _notificationContext;
        private CompanyService _companyService;

        public CompanyServiceTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _notificationContext = new NotificationContext();
            _companyService = new CompanyService(_mockUserService.Object, _mockCompanyRepository.Object, _notificationContext);
        }

        [SetUp]
        public void SetUp()
        {
            _mockUserService = new Mock<IUserService>();
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _notificationContext.Clear();
            _companyService = new CompanyService(_mockUserService.Object, _mockCompanyRepository.Object, _notificationContext);
        }

        [Test]
        public void AddCompany_ShouldInvokeTheHasUserMethodOnhce()
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
            var company = new CompanyBuilder().ByRequest(request).Generate();
            var notification = new Notification("Email", "Email já cadastrado");
            _mockUserService.Setup(method => method.HasUser(It.Is<string>(prop => prop == request.Email))).ReturnsAsync(true);

            _ = _companyService.AddAsync(request);

            _notificationContext.Notifications.Should().ContainEquivalentOf(notification);
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

