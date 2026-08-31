using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Registration.DTOs;

public class ShopProfileDto
{
    public Guid Id { get; set; }

    // Step 1
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalCode { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ShopName { get; set; }
    public ShopCategoryDto? ShopCategory { get; set; }

    // Step 2
    public Guid? AvatarFileId { get; set; }
    public string? AvatarPath { get; set; }
    public Guid? CoverFileId { get; set; }
    public string? CoverPath { get; set; }
    public string? TextAddress { get; set; }
    public Guid? ProvinceId { get; set; }
    public Guid? CityId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // Step 3
    public string? ShabaNumber { get; set; }
    public string? ReturnPolicy { get; set; }
    public List<ShopWorkingHourDto> WorkingHours { get; set; } = new();

    // Status
    public ShopRegistrationStep CurrentStep { get; set; }
    public ShopVerificationStatus VerificationStatus { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? SubmittedAt { get; set; }
}