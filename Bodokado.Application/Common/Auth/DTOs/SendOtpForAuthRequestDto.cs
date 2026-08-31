namespace Bodokado.Application.Common.Auth.DTOs;

public class SendOtpForAuthRequestDto
{
    public string PhoneNumber { get; set; } = default!;
}
