using FluentAssertions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.ValuesObjects;
using MeAgendaAi.Domains.Interfaces.Repositories;
using MeAgendaAi.Domains.Interfaces.Services;
using MeAgendaAi.Services.UserServices;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services
{
    public class UserServiceTest
    {

        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService userService;

        public UserServiceTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            userService = new UserService(_mockUserRepository.Object);
        }

        [Test]
        public async Task ShouldCheckIfUserExistsAndReturnTrue()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var email = physicalPerson.Email.Email;
            _mockUserRepository
                .Setup(method => method.GetByEmail(It.Is<string>(prop => prop == email)))
                .ReturnsAsync(physicalPerson);

            var response = await userService.HasUser(email);

            response.Should().BeTrue();
        }

        [Test]
        public async Task ShouldCheckIfUserExistsAndReturnFalse()
        {
            var email = new EmailObjectBulder().Generate();
            _mockUserRepository
                .Setup(method => method.GetByEmail(It.Is<string>(prop => prop == email.Email)));

            var response = await userService.HasUser(email.Email);

            response.Should().BeFalse();
        }

        [Test]
        public async Task ShouldCheckIfUserExistsAndCallMethodGetByEmailOfUserRepositoryOnce()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var email = physicalPerson.Email.Email;
            _mockUserRepository
                .Setup(method => method.GetByEmail(It.Is<string>(prop => prop == email)))
                .ReturnsAsync(physicalPerson);

            var response = await userService.HasUser(email);

            _mockUserRepository.Verify(method => method.GetByEmail(It.Is<string>(prop => prop == email)), Times.Once());
        }
    }
}
