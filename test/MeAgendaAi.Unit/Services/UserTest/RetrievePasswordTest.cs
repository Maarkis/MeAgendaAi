using FluentAssertions;
using MeAgendaAi.Application.Notification;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Infra.MailJet;
using MeAgendaAi.Services.UserServices;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services.UserTest
{
    public class RetrievePasswordTest
    {
        private readonly AutoMocker _mocker;
        private readonly UserService _userService;

        private const string ActionType = "UserService";
        private const int ExpirationTimeTokenInSeconds = 3600;

        public RetrievePasswordTest()
        {
            _mocker = new AutoMocker();
            _userService = _mocker.CreateInstance<UserService>();
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<IUserRepository>().Reset();
            _mocker.Get<NotificationContext>().Clear();
            _mocker.GetMock<ILogger<UserService>>().Reset();
            _mocker.GetMock<IDistributedCacheRepository>().Reset();
            _mocker.GetMock<IEmailService>().Reset();
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldCallMethodGetByEmailOnce()
        {
            var emailRequest = "test-email@hotmail.com";
            var user = new UserBuilder().WithEmail(emailRequest).Generate();
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)))
                .ReturnsAsync(user);

            _ = await _userService.RetrievePasswordAsync(emailRequest);
            
            _mocker.GetMock<IUserRepository>()
                .Verify(verify => verify.GetEmailAsync(It.Is<string>(email => email == emailRequest)), Times.Once);
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldAddNotificationWhenNotFindUser()
        {
            var emailRequest = "test-email@hotmail.com";
            var notification = new Notification("User", "User not found.");
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)));

            _ = await _userService.RetrievePasswordAsync(emailRequest);

            _mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldGenerateAnErrorLogWhenNotFindUser()
        {
            var emailRequest = "test-email@hotmail.com";
            var logMessageExpected = $"[{ActionType}/RetrievePasswordAsync] User {emailRequest} not found.";
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)));

            _ = await _userService.RetrievePasswordAsync(emailRequest);

            _mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Error, logMessageExpected);
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldCallMethodSetAsyncOnce()
        {
            var emailRequest = "test-email@hotmail.com";
            var user = new UserBuilder().WithEmail(emailRequest).Generate();
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)))
                .ReturnsAsync(user);

            _ = await _userService.RetrievePasswordAsync(emailRequest);

            _mocker.GetMock<IDistributedCacheRepository>()
                .Verify(verify => verify.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()), Times.Once());
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldCallMethodSendPasswordRecoveryEmailOnce()
        {
            var emailRequest = "test-email@hotmail.com";
            var user = new UserBuilder().WithEmail(emailRequest).Generate();
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == emailRequest)))
                .ReturnsAsync(user);

            _ = await _userService.RetrievePasswordAsync(emailRequest);

            _mocker.GetMock<IEmailService>()
                .Verify(verify => verify.SendPasswordRecoveryEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }
    }
}