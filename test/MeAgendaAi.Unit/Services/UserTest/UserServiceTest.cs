using Bogus;
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

namespace MeAgendaAi.Unit.Services.UserTest
{
    public class UserServiceTest
    {

        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Test]
        public async Task HasUser_ShouldCheckIfUserExistsAndReturnTrue()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var email = physicalPerson.Email.Email;
            _mockUserRepository
                .Setup(method => method.GetByEmail(It.Is<string>(prop => prop == email)))
                .ReturnsAsync(physicalPerson);

            var response = await _userService.HasUser(email);

            response.Should().BeTrue();
        }

        [Test]
        public async Task HasUser_ShouldCheckIfUserExistsAndReturnFalse()
        {
            var email = new EmailObjectBuilder().Generate();
            _mockUserRepository
                .Setup(method => method.GetByEmail(It.Is<string>(prop => prop == email.Email)));

            var response = await _userService.HasUser(email.Email);

            response.Should().BeFalse();
        }

        [Test]
        public async Task HasUser_ShouldCheckIfUserExistsAndCallMethodGetByEmailOfUserRepositoryOnce()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            var email = physicalPerson.Email.Email;
            _mockUserRepository
                .Setup(method => method.GetByEmail(It.Is<string>(prop => prop == email)))
                .ReturnsAsync(physicalPerson);

            var response = await _userService.HasUser(email);

            _mockUserRepository.Verify(method => method.GetByEmail(It.Is<string>(prop => prop == email)), Times.Once());
        }


        [Test]
        public void SamePassword_PasswordAndConfirmPasswordShouldTheSameAndReturnTrue()
        {
            const int LENGTH_MININUM_PASSWORD = 06;
            const int LENGTH_MAXIMUM_PASSWORD = 32;
            var faker = new Faker();
            var password = faker.Internet.Password(faker.Random.Int(LENGTH_MININUM_PASSWORD, LENGTH_MAXIMUM_PASSWORD));
            var confirmPassword = password;

            var response = _userService.SamePassword(password, confirmPassword);

            response.Should().BeTrue();
        }

        [Test]
        public void SamePassword_PasswordAndConfirmPasswordShouldNotSameAndReturnFalse()
        {
            const int LENGTH_MININUM_PASSWORD = 06;
            const int LENGTH_MAXIMUM_PASSWORD = 32;
            var faker = new Faker();
            var password = faker.Internet.Password(faker.Random.Int(LENGTH_MININUM_PASSWORD, LENGTH_MAXIMUM_PASSWORD));
            var confirmPassword = faker.Internet.Password(faker.Random.Int(LENGTH_MININUM_PASSWORD, LENGTH_MAXIMUM_PASSWORD));

            var response = _userService.SamePassword(password, confirmPassword);

            response.Should().BeFalse();
        }

        [Test]
        public void NotSamePassword_PasswordAndConfirmPasswordShouldNotSameAndReturnTrue()
        {
            const int LENGTH_MININUM_PASSWORD = 06;
            const int LENGTH_MAXIMUM_PASSWORD = 32;
            var faker = new Faker();
            var password = faker.Internet.Password(faker.Random.Int(LENGTH_MININUM_PASSWORD, LENGTH_MAXIMUM_PASSWORD));
            var confirmPassword = faker.Internet.Password(faker.Random.Int(LENGTH_MININUM_PASSWORD, LENGTH_MAXIMUM_PASSWORD));

            var response = _userService.NotSamePassword(password, confirmPassword);

            response.Should().BeTrue();
        }

        [Test]
        public void NotSamePassword_PasswordAndConfirmPasswordShouldSameAndReturnFalse()
        {
            const int LENGTH_MININUM_PASSWORD = 06;
            const int LENGTH_MAXIMUM_PASSWORD = 32;
            var faker = new Faker();
            var password = faker.Internet.Password(faker.Random.Int(LENGTH_MININUM_PASSWORD, LENGTH_MAXIMUM_PASSWORD));
            var confirmPassword = password;

            var response = _userService.NotSamePassword(password, confirmPassword);

            response.Should().BeFalse();
        }
    }
}
