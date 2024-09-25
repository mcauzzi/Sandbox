namespace SandboxAuthenticationInterfaces;

public interface ISecretsProvider
{
    public string SymmetricKey { get; }
    public string GetIssuer { get; }
    public string GetAudience { get; }
}