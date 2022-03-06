using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace MeAgendaAi.Infra.Cryptography
{
    public class Encrypt
    {
        private const int HasingInterationsCount = 10000;
        private const int BytesRequested = (256 / 8);

        public static string EncryptString(string str, string salt)
        {
            byte[] byteSalt = GenerateSalt(salt);
            byte[] bytes = ComputeHash(str, byteSalt);

            return ConvertBytes(bytes);
        }

        public static string GenerateToken(int bytesRequested = BytesRequested)
        {
            var randomNumber = new byte[bytesRequested];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return ConvertBytes(randomNumber).Replace("+", "");
        }

        private static string ConvertBytes(byte[] bytes) => Convert.ToBase64String(bytes);

        private static byte[] ComputeHash(
            string str, byte[] salt,
            int iterations = HasingInterationsCount, int bytesRequest = BytesRequested) =>
                KeyDerivation.Pbkdf2(
                    password: str, salt: salt, prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: iterations, numBytesRequested: bytesRequest);

        private static byte[] GenerateSalt(string salt) => Encoding.ASCII.GetBytes(salt);
    }
}