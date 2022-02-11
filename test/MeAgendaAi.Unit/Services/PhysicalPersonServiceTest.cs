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
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services
{
    public class PhysicalPersonServiceTest
    {
        private Mock<IUserService> _mockUserService;
        private Mock<IPhysicalPersonRepository> _mockPhysicalPersonRepository;
        private NotificationContext _notificationContext;
        private PhysicalPersonService _physicalPersonService;

        public PhysicalPersonServiceTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockPhysicalPersonRepository = new Mock<IPhysicalPersonRepository>();
            _notificationContext = new NotificationContext();
            _physicalPersonService = new PhysicalPersonService(_mockUserService.Object, _mockPhysicalPersonRepository.Object, _notificationContext);
        }

        [SetUp]
        public void SetUp()
        {
            _notificationContext.Clear();
            _mockUserService = new Mock<IUserService>();
            _mockPhysicalPersonRepository = new Mock<IPhysicalPersonRepository>();
            _physicalPersonService = new PhysicalPersonService(_mockUserService.Object, _mockPhysicalPersonRepository.Object, _notificationContext);
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
    }
}
