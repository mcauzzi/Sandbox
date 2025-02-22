using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthContextEfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SandboxAuthenticationInterfaces;

namespace SandboxAuthentication;

public class TokenService:ITokenService
{
    public AuthContext      Context         { get; }
    public ISecretsProvider SecretsProvider { get; }

    public TokenService(AuthContext context, ISecretsProvider secretsProvider)
    {
        Context              = context;
        SecretsProvider = secretsProvider;
    }
    
    public string GenerateToken(string username, IList<string> roles)
    {
        var claims = new List<Claim>
                     {
                         new(ClaimTypes.Name, username),
                         new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretsProvider.SymmetricKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer: SecretsProvider.GetIssuer, audience: SecretsProvider.GetAudience,
                                         claims: claims, expires: DateTime.UtcNow.AddMinutes(30),
                                         signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task<string> GenerateRefreshToken(string username)
    {
        // Create a 32-byte array to hold cryptographically secure random bytes
        var randomNumber = new byte[32];

        // Use a cryptographically secure random number generator 
        // to fill the byte array with random values
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);
        Context.RefreshTokens.Add(new()
                                  {
                                      Expiration = DateTime.UtcNow.AddSeconds(3600),
                                      Token      = refreshToken,
                                      User       = username
                                  });
        await Context.RefreshTokens.Where(x => x.User == username).ExecuteDeleteAsync();
        await Context.SaveChangesAsync();
        // Convert the random bytes to a base64 encoded string 
        return refreshToken;
    }
    
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        // Define the token validation parameters used to validate the token.
        var tokenValidationParameters = new TokenValidationParameters
                                        {
                                            ValidateIssuer   = true,
                                            ValidateAudience = true,
                                            ValidAudience    = SecretsProvider.GetAudience,
                                            ValidIssuer      = SecretsProvider.GetIssuer,
                                            ValidateLifetime = false, 
                                            ClockSkew        = TimeSpan.Zero,
                                            IssuerSigningKey = new SymmetricSecurityKey
                                                (Encoding.UTF8.GetBytes(SecretsProvider.SymmetricKey))
                                        };

        var tokenHandler = new JwtSecurityTokenHandler();

        // Validate the token and extract the claims principal and the security token.
        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

        // Cast the security token to a JwtSecurityToken for further validation.
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        // Ensure the token is a valid JWT and uses the HmacSha256 signing algorithm.
        // If no throw new SecurityTokenException
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals
                (SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        // return the principal
        return principal;
    }
}