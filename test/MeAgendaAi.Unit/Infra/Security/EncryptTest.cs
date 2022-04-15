using FluentAssertions;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Infra.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NUnit.Framework;
using System;
using System.Text;

namespace MeAgendaAi.Unit.Infra.Security
{
    public class EncryptTest
    {
        [Test]
        public void Encrypt_ShouldEncryptPasswordRandomic()
        {
            var password = PasswordBuilder.Generate();
            var salt = Guid.NewGuid();
            var saltBytes = Encoding.ASCII.GetBytes(salt.ToString());
            var passwordHash = KeyDerivation.Pbkdf2(
                password: password, salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: 258 / 8);
            var passwordExpected = Convert.ToBase64String(passwordHash);

            var passwordEncrypted = Encrypt.EncryptString(password, salt.ToString());

            passwordEncrypted.Should().Be(passwordExpected.ToString());
        }
    }
}