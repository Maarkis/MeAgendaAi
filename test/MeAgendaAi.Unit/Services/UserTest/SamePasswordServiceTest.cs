using FluentAssertions;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Services;
using Moq.AutoMock;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Services.UserTest;

public class SamePasswordServiceTest
{
	private readonly UserService _userService;

	public SamePasswordServiceTest()
	{
		var mocker = new AutoMocker();
		_userService = mocker.CreateInstance<UserService>();
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