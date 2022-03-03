using FluentAssertions;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Services.UserServices;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Services.UserTest
{
    public class SamePasswordServiceTest
    {        
        private readonly UserService _userService;

        public SamePasswordServiceTest()
        {
            var _mocker = new AutoMocker();
            _userService = _mocker.CreateInstance<UserService>();
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
    }
}
