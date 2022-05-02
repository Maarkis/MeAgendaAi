using System.Text;
using Bogus;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MeAgendaAi.Common.Builder.Common;

public static class PasswordBuilder
{
	private const int LengthPasswordMinimum = 06;
	private const int LengthPasswordMaximum = 32;

	public static string Generate(int lengthMinimum = LengthPasswordMinimum, int lengthMaximum = LengthPasswordMaximum)
	{
		var faker = new Faker();
		return faker.Internet.Password(faker.Random.Int(lengthMinimum, lengthMaximum));
	}

	public static string Encrypt(string password, Guid? guid = null)
	{
		var salt = guid ?? Guid.NewGuid();
		var saltBytes = Encoding.ASCII.GetBytes(salt.ToString());
		var passwordHash = KeyDerivation.Pbkdf2(
			password, saltBytes,
			KeyDerivationPrf.HMACSHA256, 10000, 258 / 8);
		var passwordEncrypt = Convert.ToBase64String(passwordHash);

		return passwordEncrypt;
	}
}