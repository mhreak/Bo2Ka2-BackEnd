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