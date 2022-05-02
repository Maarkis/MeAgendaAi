using System;
using System.Text;
using FluentAssertions;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Infra.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Infra.Security;

public class DecryptTest
{
	[Test]
	public void Decrypt_ShouldDescryptPasswordRandomic()
	{
		var password = PasswordBuilder.Generate();
		var salt = Guid.NewGuid();
		var saltBytes = Encoding.ASCII.GetBytes(salt.ToString());
		var passwordHash = KeyDerivation.Pbkdf2(
			password, saltBytes,
			KeyDerivationPrf.HMACSHA256, 10000, 258 / 8);
		var passwordEncrypt = Convert.ToBase64String(passwordHash);

		var result = Decrypt.IsValidPassword(password, salt.ToString(), passwordEncrypt);

		result.Should().BeTrue();
	}

	[Test]
	public void Decrypt_ShouldDescryptWrongPasswordRandomicAndReturnFalse()
	{
		var password = PasswordBuilder.Generate();
		var passwordWrong = PasswordBuilder.Generate();
		var salt = Guid.NewGuid();
		var saltBytes = Encoding.ASCII.GetBytes(salt.ToString());
		var passwordHash = KeyDerivation.Pbkdf2(
			password, saltBytes,
			KeyDerivationPrf.HMACSHA256, 10000, 258 / 8);
		var passwordEncrypt = new StringBuilder();
		foreach (var bit in passwordHash)
			passwordEncrypt.Append(bit);

		var result = Decrypt.IsValidPassword(passwordWrong, salt.ToString(), passwordEncrypt.ToString());

		result.Should().BeFalse();
	}
}