using Bodokado.Domain.Common;
using Bodokado.Domain.Entities.Locations;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Enums;

namespace Bodokado.Domain.Entities.Shops;

public class Shop : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Step 1 - اطلاعات پایه (هویتی + کسب و کار)
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalCode { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? ShopName { get; set; }
    public Guid? ShopCategoryId { get; set; }
    public ShopCategory? ShopCategory { get; set; }

    // Step 2 - جزئیات فروشگاه
    public Guid? AvatarFileId { get; set; }
    public FileAsset? AvatarFile { get; set; }
    public Guid? CoverFileId { get; set; }
    public FileAsset? CoverFile { get; set; }
    public string? TextAddress { get; set; }
    public Guid? ProvinceId { get; set; }
    public Province? Province { get; set; }
    public Guid? CityId { get; set; }
    public City? City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // Step 3 - تایید نهایی
    public string? ShabaNumber { get; set; }
    public string? ReturnPolicy { get; set; }

    public List<ShopWorkingHour> WorkingHours { get; set; } = new();

    // وضعیت ثبت‌نام / بررسی
    public ShopRegistrationStep CurrentStep { get; set; } = ShopRegistrationStep.BasicInfo;
    public ShopVerificationStatus VerificationStatus { get; set; } = ShopVerificationStatus.Draft;
    public string? RejectionReason { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}