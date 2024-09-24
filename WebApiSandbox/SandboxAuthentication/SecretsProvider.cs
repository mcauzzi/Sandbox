using Microsoft.Extensions.Options;
using SandboxAuthenticationInterfaces;
using SandboxConfigurations;

namespace SandboxAuthentication;

public class SecretsProvider:ISecretsProvider
{
    private readonly AuthConfig _configuration;
    public SecretsProvider(IOptions<AuthConfig> configuration)
    {
        _configuration = configuration.Value;
    }
    public string GetSymmetricKey => _configuration.SymmetricKey;
    public string GetIssuer => _configuration.Issuer;
    public string GetAudience => _configuration.Audience;
}