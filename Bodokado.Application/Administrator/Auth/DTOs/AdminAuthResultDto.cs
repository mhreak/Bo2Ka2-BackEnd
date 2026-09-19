namespace Bodokado.Application.Administrator.Auth.DTOs;

public class AdminAuthResultDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
