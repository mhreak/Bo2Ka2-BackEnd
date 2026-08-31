namespace Bodokado.Application.Common.Auth.DTOs;

public class GoogleAuthResultDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public bool IsNewUser { get; set; }
    public bool IsProfileCompleted { get; set; }
}
