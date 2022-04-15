using FluentAssertions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Services;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services.UserTest
{
    public class GetByEmailServiceTest
    {
        private readonly AutoMocker _mocker;
        private readonly UserService _userService;

        public GetByEmailServiceTest()
        {
            _mocker = new AutoMocker();
            _userService = _mocker.CreateInstance<UserService>();
        }

        [SetUp]
        public void SetUp()
        {
            _mocker.GetMock<IUserRepository>().Reset();
        }

        [Test]
        public async Task GetByEmailAsync_ShouldMethodGetByEmailInvokedOnlyOnce()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var email = physicalPerson.Email.Address;
            _mocker.GetMock<IUserRepository>()
                .Setup(method => method.GetEmailAsync(It.Is<string>(prop => prop == email)))
                .ReturnsAsync(physicalPerson);

            _ = await _userService.GetByEmailAsync(email);

            _mocker.GetMock<IUserRepository>()
                .Verify(verify => verify.GetEmailAsync(It.Is<string>(prop => prop == email)), Times.Once());
        }

        [Test]
        public async Task GetByEmailAsync_ShouldGetByEmailReturnAUser()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var email = physicalPerson.Email.Address;
            _mocker.GetMock<IUserRepository>()
                .Setup(method => method.GetEmailAsync(It.Is<string>(prop => prop == email)))
                .ReturnsAsync(physicalPerson);

            var response = await _userService.GetByEmailAsync(email);

            response.Should().BeEquivalentTo(physicalPerson);
        }

        [Test]
        public async Task GetByEmailAsync_ShouldGetByEmailReturnNullWhenNotFindAUser()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var email = physicalPerson.Email.Address;
            _mocker.GetMock<IUserRepository>()
                .Setup(method => method.GetEmailAsync(It.Is<string>(prop => prop == email)));

            var response = await _userService.GetByEmailAsync(email);

            response.Should().BeNull();
        }
    }
}