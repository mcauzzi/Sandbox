namespace SandboxAuthenticationInterfaces;

public interface ISecretsProvider
{
    public string GetSymmetricKey { get; }
    public string GetIssuer { get; }
    public string GetAudience { get; }
}