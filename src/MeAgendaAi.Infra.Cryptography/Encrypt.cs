using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace MeAgendaAi.Infra.Cryptography
{
    public class Encrypt
    {
        private const int HASING_INTERATIONS_COUNT = 10000;
        private const int BYTES_REQUESTED = (256 / 8);

        public static string EncryptString(string str, string salt)
        {
            byte[] byteSalt = GenerateSalt(salt);
            byte[] bytes = ComputeHash(str, byteSalt);

            return ConvertBytes(bytes);
        }

        private static string ConvertBytes(byte[] passwordBytes) => Convert.ToBase64String(passwordBytes);

        private static byte[] ComputeHash(
            string str, byte[] salt,
            int iterations = HASING_INTERATIONS_COUNT, int bytesRequest = BYTES_REQUESTED) =>
                KeyDerivation.Pbkdf2(
                    password: str, salt: salt, prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: iterations, numBytesRequested: bytesRequest);

        private static byte[] GenerateSalt(string salt) => Encoding.ASCII.GetBytes(salt);
    }
}
