using System;
using System.Text;
using FluentAssertions;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Infra.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NUnit.Framework;

namespace MeAgendaAi.Unit.Infra.Security;

public class EncryptTest
{
	[Test]
	public void Encrypt_ShouldEncryptPasswordRandomic()
	{
		var password = PasswordBuilder.Generate();
		var salt = Guid.NewGuid();
		var saltBytes = Encoding.ASCII.GetBytes(salt.ToString());
		var passwordHash = KeyDerivation.Pbkdf2(
			password, saltBytes,
			KeyDerivationPrf.HMACSHA256, 10000, 258 / 8);
		var passwordExpected = Convert.ToBase64String(passwordHash);

		var passwordEncrypted = Encrypt.EncryptString(password, salt.ToString());

		passwordEncrypted.Should().Be(passwordExpected);
	}
}