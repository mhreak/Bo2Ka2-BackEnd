// Domain/Enums/Organization/GiftCampaignConstraintType.cs
namespace Bodokado.Domain.Enums;

public enum GiftCampaignConstraintType : short
{
    /// <summary>محدودیت روی قیمت</summary>
    PriceLimit = 1,

    /// <summary>محدودیت روی دسته‌بندی‌(های) خاص</summary>
    CategoryLimit = 2,

    /// <summary>محدودیت روی فروشگاه‌(های) خاص</summary>
    ShopLimit = 3,

    /// <summary>محدودیت روی محصول‌(های) خاص</summary>
    ProductLimit = 4
}