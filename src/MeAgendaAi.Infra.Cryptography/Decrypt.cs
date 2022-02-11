namespace MeAgendaAi.Infra.Cryptography
{
    public class Decrypt
    {
        public static bool CompareComputeHash(string password, string salt, string encryptedPassword) => 
            Encrypt.EncryptString(password, salt).Equals(encryptedPassword);
    }
}
