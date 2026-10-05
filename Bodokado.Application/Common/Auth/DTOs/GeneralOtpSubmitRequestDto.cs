namespace Bodokado.Application.Common.Auth.DTOs;

public class GeneralOtpSubmitRequestDto
{
    public string Mobile { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}