using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EfCoreContext;
using Microsoft.IdentityModel.Tokens;
using SandboxAuthenticationInterfaces;

namespace SandboxAuthentication;

public class TokenService:ITokenService
{
    public SandboxContext   Context         { get; }
    public ISecretsProvider SecretsProvider { get; }

    public TokenService(SandboxContext context, ISecretsProvider secretsProvider)
    {
        Context              = context;
        SecretsProvider = secretsProvider;
    }
    public string GenerateToken(string username)
    {
        var claims = new[]
                     {
                         new Claim(JwtRegisteredClaimNames.Sub, username),
                         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                     };

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretsProvider.GetSymmetricKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer: SecretsProvider.GetIssuer,
                                         audience: SecretsProvider.GetAudience,
                                         claims: claims,
                                         expires: DateTime.Now.AddMinutes(30),
                                         signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}