using MeAgendaAi.Domains.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace MeAgendaAi.Infra.JWT
{
    public interface IJSONWebTokenService
    {
        string GenerateToken(User user);

        string? Validate(string token);
    }

    public class JWTService : IJSONWebTokenService
    {
        private readonly SigningConfiguration _signingConfiguration = default!;
        private readonly TokenConfiguration _tokenConfiguration = default!;
        private readonly ILogger<JWTService> _logger;
        private static JwtSecurityTokenHandler Handler => new();

        private const string ActionType = "JWTService";

        public JWTService(
            IOptions<TokenConfiguration> optionsTokenConfiguration,
            SigningConfiguration signingConfiguration,
            ILogger<JWTService> logger) =>
            (_tokenConfiguration, _signingConfiguration, _logger) = (optionsTokenConfiguration.Value, signingConfiguration, logger);

        public string GenerateToken(User user)
        {
            _logger.LogInformation("[{ActionType}/GenerateToken] Starting token generation process.", ActionType);

            var createdDate = DateTime.Now;
            var expirationDate = createdDate.AddSeconds(_tokenConfiguration.Seconds);

            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email.Email),
                new Claim(ClaimTypes.Expiration, expirationDate.ToString()),
            };
            var claimsIdentity = new ClaimsIdentity(new GenericIdentity(user.Id.ToString()), claims);

            return CreateToken(claimsIdentity, createdDate, expirationDate);
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

        public string? Validate(string token)
        {
            _ = Handler.ValidateToken(token, ValidationParameters(), out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
        }

        private TokenValidationParameters ValidationParameters() => new()
        {
            ValidateLifetime = _tokenConfiguration.ValidateIssuer,
            ValidateAudience = _tokenConfiguration.ValidateAudience,
            ValidateIssuer = _tokenConfiguration.ValidateIssuer,
            ValidIssuer = _tokenConfiguration.Issuer,
            ValidAudience = _tokenConfiguration.Audience,
            IssuerSigningKey = _signingConfiguration.Key
        };
    }
}