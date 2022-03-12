using FluentAssertions;
using MeAgendaAi.Common;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Repositories.Cache;
using MeAgendaAi.Infra.MailJet;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
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
            const string EmailRequest = "test-email@hotmail.com";
            var user = new UserBuilder().WithEmail(EmailRequest).Generate();
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == EmailRequest)))
                .ReturnsAsync(user);

            _ = await _userService.RetrievePasswordAsync(EmailRequest);

            _mocker.GetMock<IUserRepository>()
                .Verify(verify => verify.GetEmailAsync(It.Is<string>(email => email == EmailRequest)), Times.Once);
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldAddNotificationWhenNotFindUser()
        {
            const string EmailRequest = "test-email@hotmail.com";
            var notification = new Notification("User", "User not found.");
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == EmailRequest)));

            _ = await _userService.RetrievePasswordAsync(EmailRequest);

            _mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notification);
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldGenerateAnErrorLogWhenNotFindUser()
        {
            const string EmailRequest = "test-email@hotmail.com";
            var logMessageExpected = $"[{ActionType}/RetrievePasswordAsync] User {EmailRequest} not found.";
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == EmailRequest)));

            _ = await _userService.RetrievePasswordAsync(EmailRequest);

            _mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Error, logMessageExpected);
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldCallMethodSetAsyncOnce()
        {
            const string EmailRequest = "test-email@hotmail.com";
            var user = new UserBuilder().WithEmail(EmailRequest).Generate();
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == EmailRequest)))
                .ReturnsAsync(user);

            _ = await _userService.RetrievePasswordAsync(EmailRequest);

            _mocker.GetMock<IDistributedCacheRepository>()
                .Verify(verify => verify.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()), Times.Once());
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldMethodSetAsyncWithValuesCorrectly()
        {
            var id = Guid.NewGuid();
            const string EmailRequest = "test-email@hotmail.com";
            var user = new UserBuilder().WithId(id).WithEmail(EmailRequest).Generate();
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == EmailRequest)))
                .ReturnsAsync(user);

            _ = await _userService.RetrievePasswordAsync(EmailRequest);

            _mocker.GetMock<IDistributedCacheRepository>()
                .Verify(verify => verify.SetAsync(
                    It.IsAny<string>(),
                    It.Is<string>(value => value == id.ToString()),
                    It.Is<double>(expireInSeconds => expireInSeconds == ExpirationTimeTokenInSeconds)), Times.Once());
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldCallMethodSendPasswordRecoveryEmailOnce()
        {
            const string EmailRequest = "test-email@hotmail.com";
            var user = new UserBuilder().WithEmail(EmailRequest).Generate();
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == EmailRequest)))
                .ReturnsAsync(user);

            _ = await _userService.RetrievePasswordAsync(EmailRequest);

            _mocker.GetMock<IEmailService>()
                .Verify(verify => verify.SendPasswordRecoveryEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldMethodSendPasswordRecoveryEmailWithExactyData()
        {
            const string Name = "name-expected";
            const string Surname = "surname-expected";
            const string FullNameExpected = $"{Name} {Surname}";
            const string EmailRequest = "test-email@hotmail.com";
            var user = new UserBuilder().WithNameAndSurname(Name, Surname).WithEmail(EmailRequest).Generate();
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == EmailRequest)))
                .ReturnsAsync(user);

            _ = await _userService.RetrievePasswordAsync(EmailRequest);

            _mocker.GetMock<IEmailService>()
                .Verify(verify => verify.SendPasswordRecoveryEmail(
                    It.Is<string>(fullName => fullName == FullNameExpected),
                    It.Is<string>(email => email == EmailRequest),
                    It.IsAny<string>(),
                    It.Is<int>(expirationInSeconds => expirationInSeconds == ExpirationTimeTokenInSeconds)), Times.Once());
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldGenerateAnInformationLogWhenTheEmailWasSuccessfullySent()
        {
            const bool Sent = true;
            const string EmailRequest = "test-email@hotmail.com";
            var logMessageExpected = $"[{ActionType}/RetrievePasswordAsync] Email successfully sent to {EmailRequest}.";
            var user = new UserBuilder().WithEmail(EmailRequest).Generate();
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == EmailRequest)))
                .ReturnsAsync(user);
            _mocker.GetMock<IEmailService>()
                .Setup(setup => setup.SendPasswordRecoveryEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(Sent);

            _ = await _userService.RetrievePasswordAsync(EmailRequest);

            _mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Information, logMessageExpected);
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldAddNotificationWhenTheEmailIsNotSend()
        {
            const bool NotSend = false;
            const string EmailRequest = "test-email@hotmail.com";
            var user = new UserBuilder().WithEmail(EmailRequest).Generate();
            var notificationExpected = new Notification("SendEmail", "Email not sent.");
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == EmailRequest)))
                .ReturnsAsync(user);
            _mocker.GetMock<IEmailService>()
                .Setup(setup => setup.SendPasswordRecoveryEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(NotSend);

            _ = await _userService.RetrievePasswordAsync(EmailRequest);

            _mocker.Get<NotificationContext>().Notifications.Should().ContainEquivalentOf(notificationExpected);
        }

        [Test]
        public async Task RetrievePasswordAsync_ShouldGenerateAnErrorLogWhenTheEmailNotWasSuccessfullySent()
        {
            const bool NotSend = false;
            const string EmailRequest = "test-email@hotmail.com";
            var user = new UserBuilder().WithEmail(EmailRequest).Generate();
            var logMessageExpected = $"[{ActionType}/RetrievePasswordAsync] Email not sent to {EmailRequest}.";
            _mocker.GetMock<IUserRepository>()
                .Setup(setup => setup.GetEmailAsync(It.Is<string>(email => email == EmailRequest)))
                .ReturnsAsync(user);
            _mocker.GetMock<IEmailService>()
                .Setup(setup => setup.SendPasswordRecoveryEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(NotSend);

            _ = await _userService.RetrievePasswordAsync(EmailRequest);

            _mocker.GetMock<ILogger<UserService>>().VerifyLog(LogLevel.Error, logMessageExpected);
        }
    }
}