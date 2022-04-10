using FluentAssertions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Services;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services.UserTest
{
    public class HasUserServiceTest
    {
        private readonly AutoMocker _mocker;
        private readonly UserService _userService;

        public HasUserServiceTest()
        {
            _mocker = new AutoMocker();
            _userService = _mocker.CreateInstance<UserService>();
        }

        [Test]
        public async Task HasUser_ShouldCheckIfUserExistsAndReturnTrue()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var email = physicalPerson.Email.Address;
            _mocker.GetMock<IUserRepository>()
                .Setup(method => method.GetEmailAsync(It.Is<string>(prop => prop == email)))
                .ReturnsAsync(physicalPerson);

            var response = await _userService.HasUser(email);

            response.Should().BeTrue();
        }

        [Test]
        public async Task HasUser_ShouldCheckIfUserExistsAndReturnFalse()
        {
            var email = new EmailObjectBuilder().Generate();
            _mocker.GetMock<IUserRepository>()
                .Setup(method => method.GetEmailAsync(It.Is<string>(prop => prop == email.Address)));

            var response = await _userService.HasUser(email.Address);

            response.Should().BeFalse();
        }

        [Test]
        public async Task HasUser_ShouldCheckIfUserExistsAndCallMethodGetByEmailOfUserRepositoryOnce()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var email = physicalPerson.Email.Address;
            _mocker.GetMock<IUserRepository>()
                .Setup(method => method.GetEmailAsync(It.Is<string>(prop => prop == email)))
                .ReturnsAsync(physicalPerson);

            _ = await _userService.HasUser(email);

            _mocker.GetMock<IUserRepository>().Verify(method => method.GetEmailAsync(It.Is<string>(prop => prop == email)), Times.Once());
        }
    }
}