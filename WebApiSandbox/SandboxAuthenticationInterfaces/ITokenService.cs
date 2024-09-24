namespace SandboxAuthenticationInterfaces;

public interface ITokenService
{
    public string GenerateToken(string username);
}