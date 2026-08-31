namespace Bodokado.Application.Common.Auth.DTOs;

public class SendOtpForAuthResponseDto
{
    public string? Code { get; set; }
    public int ExpiresIn { get; set; }
}
