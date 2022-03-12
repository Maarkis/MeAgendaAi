using MeAgendaAi.Domains.Entities;
using MeAgendaAi.Infra.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace MeAgendaAi.Infra.JWT
{
    public interface IJsonWebTokenService
    {
        JwtToken GenerateToken(User user);
    }

    public class JwtService : IJsonWebTokenService
    {
        private readonly SigningConfiguration _signingConfiguration = default!;
        private readonly TokenConfiguration _tokenConfiguration = default!;
        private readonly ILogger<JwtService> _logger;
        private static JwtSecurityTokenHandler Handler => new();

        private const string ActionType = "JWTService";

        public JwtService(
            IOptions<TokenConfiguration> optionsTokenConfiguration,
            SigningConfiguration signingConfiguration,
            ILogger<JwtService> logger) =>
            (_tokenConfiguration, _signingConfiguration, _logger) = (optionsTokenConfiguration.Value, signingConfiguration, logger);

        public JwtToken GenerateToken(User user)
        {
            _logger.LogInformation("[{ActionType}/GenerateToken] Starting token generation process.", ActionType);

            var createdDate = DateTime.Now;
            var expirationDate = createdDate.AddSeconds(_tokenConfiguration.ExpirationTimeInSeconds);

            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email.Email),
                new Claim(ClaimTypes.Expiration, expirationDate.ToString()),
            };
            var claimsIdentity = new ClaimsIdentity(new GenericIdentity(user.Id.ToString()), claims);

            return new()
            {
                Token = CreateToken(claimsIdentity, createdDate, expirationDate),
                RefreshToken = CreateRefreshToken(createdDate)
            };
        }

        private RefreshToken CreateRefreshToken(DateTime createdDate)
        {
            _logger.LogInformation("[{ActionType}/GenerateToken] Starting refresh token generation process.", ActionType);

            return new()
            {
                Token = Encrypt.GenerateToken(),
                ExpiresIn = createdDate.AddSeconds(_tokenConfiguration.RefreshTokenExpirationTimeInSeconds),
            };
        }

        private string CreateToken(ClaimsIdentity claimsIdentity, DateTime createdDate, DateTime expirationDate)
        {
            _logger.LogInformation("[{ActionType}/CreateToken] Creating Token for User.", ActionType);

            var securityToken = Handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfiguration.Issuer,
                Audience = _tokenConfiguration.Audience,
                SigningCredentials = _signingConfiguration.SigningCredentials,
                Subject = claimsIdentity,
                NotBefore = createdDate,
                Expires = expirationDate,
            });

            var token = Handler.WriteToken(securityToken);
            _logger.LogInformation("[{ActionType}/CreateToken] Token created successfully.", ActionType);
            return token;
        }
    }
}