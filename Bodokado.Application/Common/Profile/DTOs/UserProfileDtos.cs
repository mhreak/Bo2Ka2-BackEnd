using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.Profile.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim();

    public string? NationalCode { get; set; }
    public string? Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ShamsiBirthDate { get; set; }
    public Gender? Gender { get; set; }

    public string? Address { get; set; }
    public Guid? CityId { get; set; }
    public string? CityName { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public Guid? AvatarFileId { get; set; }
    public string? AvatarPath { get; set; }

    /// <summary>از User.WalletCredit</summary>
    public long WalletCredit { get; set; }

    public bool IsActive { get; set; }
    public bool HasPassword { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}

public class UpdateProfileRequestDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalCode { get; set; }
    public string? Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ShamsiBirthDate { get; set; }
    public Gender? Gender { get; set; }
    public string? Address { get; set; }
    public Guid? CityId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Guid? AvatarFileId { get; set; }
}
public class SetPasswordRequestDto
{
    public string NewPassword { get; set; } = string.Empty;
}
