namespace Bodokado.Application.App.Auth.DTOs;

public class SubmitOtpRequestDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
