using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace MeAgendaAi.Infra.JWT
{
    public class SigningConfiguration
    {
        public SecurityKey Key { get; set; }
        public SigningCredentials SigningCredentials { get; set; }

        private const int dwKeySize = 2048;

        public SigningConfiguration()
        {
            using (var provider = new RSACryptoServiceProvider(dwKeySize))
            {
                Key = new RsaSecurityKey(provider.ExportParameters(true));
            }

            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
        }
    }
}