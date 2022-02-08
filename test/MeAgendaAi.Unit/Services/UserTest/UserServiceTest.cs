using FluentAssertions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Services.UserServices;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services.UserTest
{
    public class UserServiceTest
    {
        private readonly AutoMocker _mocker;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _mocker = new AutoMocker();
            _userService = _mocker.CreateInstance<UserService>();
        }

        [Test]
        public async Task HasUser_ShouldCheckIfUserExistsAndReturnTrue()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var email = physicalPerson.Email.Email;
            _mocker.GetMock<IUserRepository>()
                .Setup(method => method.GetByEmail(It.Is<string>(prop => prop == email)))
                .ReturnsAsync(physicalPerson);

            var response = await _userService.HasUser(email);

            response.Should().BeTrue();
        }

        [Test]
        public async Task HasUser_ShouldCheckIfUserExistsAndReturnFalse()
        {
            var email = new EmailObjectBuilder().Generate();
            _mocker.GetMock<IUserRepository>()
                .Setup(method => method.GetByEmail(It.Is<string>(prop => prop == email.Email)));

            var response = await _userService.HasUser(email.Email);

            response.Should().BeFalse();
        }

        [Test]
        public async Task HasUser_ShouldCheckIfUserExistsAndCallMethodGetByEmailOfUserRepositoryOnce()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var email = physicalPerson.Email.Email;
            _mocker.GetMock<IUserRepository>()
                .Setup(method => method.GetByEmail(It.Is<string>(prop => prop == email)))
                .ReturnsAsync(physicalPerson);

            var response = await _userService.HasUser(email);

            _mocker.GetMock<IUserRepository>().Verify(method => method.GetByEmail(It.Is<string>(prop => prop == email)), Times.Once());
        }

        [Test]
        public void SamePassword_PasswordAndConfirmPasswordShouldTheSameAndReturnTrue()
        {
            var password = PasswordBuilder.Generate();
            var confirmPassword = password;

            var response = _userService.SamePassword(password, confirmPassword);

            response.Should().BeTrue();
        }

        [Test]
        public void SamePassword_PasswordAndConfirmPasswordShouldNotSameAndReturnFalse()
        {
            var password = PasswordBuilder.Generate();
            var confirmPassword = PasswordBuilder.Generate();

            var response = _userService.SamePassword(password, confirmPassword);

            response.Should().BeFalse();
        }

        [Test]
        public void NotSamePassword_PasswordAndConfirmPasswordShouldNotSameAndReturnTrue()
        {
            var password = PasswordBuilder.Generate();
            var confirmPassword = PasswordBuilder.Generate();

            var response = _userService.NotSamePassword(password, confirmPassword);

            response.Should().BeTrue();
        }

        [Test]
        public void NotSamePassword_PasswordAndConfirmPasswordShouldSameAndReturnFalse()
        {
            var password = PasswordBuilder.Generate();
            var confirmPassword = password;

            var response = _userService.NotSamePassword(password, confirmPassword);

            response.Should().BeFalse();
        }
    }
}