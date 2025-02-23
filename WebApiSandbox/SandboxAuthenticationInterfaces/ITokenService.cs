using System.Security.Claims;

namespace SandboxAuthenticationInterfaces;

public interface ITokenService
{
    public string       GenerateToken(string                userName, IList<string> roles);
    public Task<string> GenerateRefreshToken(string         userName);
    ClaimsPrincipal     GetPrincipalFromExpiredToken(string accessToken);
    Task                DeleteRefreshToken(string?          userName);
}