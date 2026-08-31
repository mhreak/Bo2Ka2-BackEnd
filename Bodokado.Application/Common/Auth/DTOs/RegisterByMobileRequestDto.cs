namespace Bodokado.Application.Common.Auth.DTOs;

public class RegisterByMobileRequestDto
{
    public string PhoneNumber { get; set; } = default!;
    public string Code { get; set; } = default!;
}
