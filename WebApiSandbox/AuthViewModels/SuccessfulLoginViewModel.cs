namespace AuthViewModels;

public class SuccessfulLoginViewModel
{
    public required string Token        { get; set; }
    public required string RefreshToken { get; set; }
}