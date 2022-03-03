using FluentAssertions;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Infra.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NUnit.Framework;
using System;
using System.Text;

namespace MeAgendaAi.Unit.Segurity
{
    public class DecryptTest
    {
        [Test]
        public void Decrypt_ShouldDescryptPasswordRandomic()
        {
            var password = PasswordBuilder.Generate();
            var salt = Guid.NewGuid();
            var saltBytes = Encoding.ASCII.GetBytes(salt.ToString());
            var passwordHash = KeyDerivation.Pbkdf2(
                password: password, salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: (258 / 8));
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
                password: password, salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: (258 / 8));
            var passwordEncrypt = new StringBuilder();
            foreach (byte bit in passwordHash)
                passwordEncrypt.Append(bit);

            var result = Decrypt.IsValidPassword(passwordWrong, salt.ToString(), passwordEncrypt.ToString());

            result.Should().BeFalse();
        }
    }
}