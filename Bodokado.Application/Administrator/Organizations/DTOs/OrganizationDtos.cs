namespace Bodokado.Application.Administrator.Organizations.DTOs;

public class CreateOrganizationRequestDto
{
    public string OrganizationName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid? LogoFileId { get; set; }
}

public class UpdateOrganizationRequestDto
{
    public string OrganizationName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid? LogoFileId { get; set; }
}

public class OrganizationDto
{
    public Guid Id { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? LogoFileId { get; set; }
    public string? LogoPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateOrganizationAdminRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class OrganizationAdminDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Guid OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public bool IsActive { get; set; }
}