namespace Bodokado.Application.Common.Auth.DTOs;

public class LoginOtpRequestDto
{
    public string PhoneNumber { get; set; } = default!;
    public string Code { get; set; } = default!;
}
