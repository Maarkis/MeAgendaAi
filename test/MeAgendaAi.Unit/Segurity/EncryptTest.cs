using Bogus;
using FluentAssertions;
using MeAgendaAi.Infra.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NUnit.Framework;
using System;
using System.Text;

namespace MeAgendaAi.Unit.Segurity
{
    public class EncryptTest
    {
        [Test]
        public void Encrypt_ShouldEncryptPasswordRandomic()
        {
            var faker = new Faker();
            var password = $"{faker.Lorem.Letter(faker.Random.Int(4, 8))}{faker.Random.Int(1, 999)}{faker.Lorem.Letter(faker.Random.Int(4, 8))}";
            var salt = Guid.NewGuid();
            var saltBytes = Encoding.ASCII.GetBytes(salt.ToString());
            var passwordHash = KeyDerivation.Pbkdf2(
                password: password, salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: (258 / 8));
            var passwordExpected = new StringBuilder();
            foreach (byte bit in passwordHash)
                passwordExpected.Append(bit.ToString());

            var passwordEncrypted = Encrypt.EncryptString(password, salt.ToString());

            passwordEncrypted.Should().Be(passwordExpected.ToString());
        }

        [Test]
        public void Descrypt_ShouldDescryptPasswordRandomic()
        {
            var faker = new Faker();
            var password = $"{faker.Lorem.Letter(faker.Random.Int(4, 8))}{faker.Random.Int(1, 999)}{faker.Lorem.Letter(faker.Random.Int(4, 8))}";
            var salt = Guid.NewGuid();
            var saltBytes = Encoding.ASCII.GetBytes(salt.ToString());

            var passwordHash = KeyDerivation.Pbkdf2(
                password: password, salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: (258 / 8));
            var passwordEncrypt = new StringBuilder();
            foreach (byte bit in passwordHash)
                passwordEncrypt.Append(bit.ToString());

            var result = Decrypt.CompareComputeHash(password, salt.ToString(), passwordEncrypt.ToString());

            result.Should().BeTrue();
        }

        [Test]
        public void Descrypt_ShouldDescryptWrongPasswordRandomicAndReturnFalse()
        {
            var faker = new Faker();
            var password = $"{faker.Lorem.Letter(faker.Random.Int(4, 8))}{faker.Random.Int(1, 999)}{faker.Lorem.Letter(faker.Random.Int(4, 8))}";
            var passwordWrong = $"{faker.Lorem.Letter(faker.Random.Int(4, 8))}{faker.Random.Int(4, 8)}{faker.Lorem.Letter(faker.Random.Int(4, 8))}";
            var salt = Guid.NewGuid();
            var saltBytes = Encoding.ASCII.GetBytes(salt.ToString());
            var passwordHash = KeyDerivation.Pbkdf2(
                password: password, salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: (258 / 8));
            var passwordEncrypt = new StringBuilder();
            foreach (byte bit in passwordHash)
                passwordEncrypt.Append(bit.ToString());

            var result = Decrypt.CompareComputeHash(passwordWrong, salt.ToString(), passwordEncrypt.ToString());

            result.Should().BeFalse();
        }
    }
}
