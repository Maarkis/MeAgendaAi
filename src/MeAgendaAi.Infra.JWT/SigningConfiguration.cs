using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MeAgendaAi.Infra.JWT;

public class SigningConfiguration
{
	private const int DwKeySize = 2048;

	public SigningConfiguration()
	{
		using (var provider = new RSACryptoServiceProvider(DwKeySize))
		{
			Key = new RsaSecurityKey(provider.ExportParameters(true));
		}

		SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
	}

	public SecurityKey Key { get; set; }
	public SigningCredentials SigningCredentials { get; set; }
}