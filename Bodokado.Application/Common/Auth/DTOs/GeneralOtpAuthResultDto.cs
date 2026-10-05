namespace Bodokado.Application.Common.Auth.DTOs;

public class GeneralOtpAuthResultDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsProfileCompleted { get; set; }
    public bool IsNewUser { get; set; }
}