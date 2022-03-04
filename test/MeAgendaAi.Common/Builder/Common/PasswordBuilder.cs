﻿using Bogus;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace MeAgendaAi.Common.Builder.Common
{
    public static class PasswordBuilder
    {
        private const int LengthPasswordMinimum = 06;
        private const int LengthPasswordMaximum = 32;

        public static string Generate(int lengthMininum = LengthPasswordMinimum, int lengthMaximium = LengthPasswordMaximum)
        {
            var faker = new Faker();
            return faker.Internet.Password(length: faker.Random.Int(lengthMininum, lengthMaximium));
        }

        public static string Encrypt(string password, Guid? guid = null)
        {
            var salt = guid ?? Guid.NewGuid();
            var saltBytes = Encoding.ASCII.GetBytes(salt.ToString());
            var passwordHash = KeyDerivation.Pbkdf2(
                password: password, salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: (258 / 8));
            var passwordEncrypt = Convert.ToBase64String(passwordHash);

            return passwordEncrypt;
        }
    }
}