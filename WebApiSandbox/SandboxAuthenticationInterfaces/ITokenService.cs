using System.Security.Claims;

namespace SandboxAuthenticationInterfaces;

public interface ITokenService
{
    public string       GenerateToken(string        username, IList<string> roles);
    public Task<string> GenerateRefreshToken(string username);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
}