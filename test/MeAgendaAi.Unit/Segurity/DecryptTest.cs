using Bogus;
using FluentAssertions;
using MeAgendaAi.Infra.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeAgendaAi.Unit.Segurity
{
    public class DecryptTest
    {

        [Test]
        public void Decrypt_ShouldDescryptPasswordRandomic()
        {
            var faker = new Faker();
            var password = $"{faker.Lorem.Letter(faker.Random.Int(4, 8))}{faker.Random.Int(1, 999)}{faker.Lorem.Letter(faker.Random.Int(4, 8))}";
            var salt = Guid.NewGuid();
            var saltBytes = Encoding.ASCII.GetBytes(salt.ToString());
            var passwordHash = KeyDerivation.Pbkdf2(
                password: password, salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: (258 / 8));
            var passwordEncrypt = Convert.ToBase64String(passwordHash);

            var result = Decrypt.CompareComputeHash(password, salt.ToString(), passwordEncrypt);

            result.Should().BeTrue();
        }

        [Test]
        public void Decrypt_ShouldDescryptWrongPasswordRandomicAndReturnFalse()
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
