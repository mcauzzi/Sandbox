namespace SandboxConfigurations;

public record AuthConfig
{
    public string SymmetricKey { get; init; }
    public string Issuer       { get; init; }
    public string Audience     { get; init; }
}
