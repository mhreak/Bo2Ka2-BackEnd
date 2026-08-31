namespace Bodokado.Application.App.Auth.DTOs;

public class SendOtpResponseDto
{
    public string? Code { get; set; }
    public int ExpiresIn { get; set; }
}
