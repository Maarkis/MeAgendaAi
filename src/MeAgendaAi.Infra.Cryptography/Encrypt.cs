using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MeAgendaAi.Infra.Cryptography;

public static class Encrypt
{
	private const int HashingInteractionsCount = 10000;
	private const int BytesRequested = 256 / 8;

	public static string EncryptString(string str, string salt)
	{
		var byteSalt = GenerateSalt(salt);
		var bytes = ComputeHash(str, byteSalt);

		return ConvertBytes(bytes);
	}

	public static string GenerateToken(int bytesRequested = BytesRequested)
	{
		var randomNumber = new byte[bytesRequested];

		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);

		return ConvertBytes(randomNumber).Replace("+", "").Replace("/", "");
	}

	private static string ConvertBytes(byte[] bytes)
	{
		return Convert.ToBase64String(bytes);
	}

	private static byte[] ComputeHash(
		string str, byte[] salt,
		int iterations = HashingInteractionsCount, int bytesRequest = BytesRequested)
	{
		return KeyDerivation.Pbkdf2(
			str, salt, KeyDerivationPrf.HMACSHA256,
			iterations, bytesRequest);
	}

	private static byte[] GenerateSalt(string salt)
	{
		return Encoding.ASCII.GetBytes(salt);
	}
}