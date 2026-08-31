namespace Bodokado.Application.Common.Auth.DTOs;

public class LoginByPasswordRequestDto
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
