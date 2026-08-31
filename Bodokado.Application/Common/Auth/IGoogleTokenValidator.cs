namespace Bodokado.Application.Common.Auth;

public interface IGoogleTokenValidator
{
    Task<GoogleUserInfo?> ValidateAsync(string idToken);
}

public class GoogleUserInfo
{
    public string GoogleId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool EmailVerified { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PictureUrl { get; set; }
}
