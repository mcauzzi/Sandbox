namespace AuthModels;

public class RefreshToken
{
    public required string User { get; set; }
    public required string Token { get; set; }
    public required DateTime Expiration { get; set; }
}