namespace MeAgendaAi.Infra.Cryptography
{
    public class Decrypt
    {
        public static bool IsValidPassword(string password, string salt, string encryptedPassword) =>
            CompareComputeHash(password, salt, encryptedPassword);

        private static bool CompareComputeHash(string password, string salt, string encryptedPassword) =>
            Encrypt.EncryptString(password, salt).Equals(encryptedPassword);
    }
}