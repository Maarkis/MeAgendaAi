﻿using FluentAssertions;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Services;
using Moq.AutoMock;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Services.UserTest;

public class NotSamePasswordServiceTest
{
	private readonly UserService _userService;

	public NotSamePasswordServiceTest()
	{
		var mocker = new AutoMocker();
		_userService = mocker.CreateInstance<UserService>();
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