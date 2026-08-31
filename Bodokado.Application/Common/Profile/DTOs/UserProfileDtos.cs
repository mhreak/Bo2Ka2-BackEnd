namespace Bodokado.Application.Common.Profile.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public bool HasPassword { get; set; }
}

public class UpdateProfileRequestDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}

public class SetPasswordRequestDto
{
    public string NewPassword { get; set; } = string.Empty;
}
