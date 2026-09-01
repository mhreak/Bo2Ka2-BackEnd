using Bodokado.Domain.Common;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Enums;

namespace Bodokado.Domain.Entities.Products;

public class Product : BaseEntity
{
    public Guid ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>وزن به گرم</summary>
    public decimal? WeightGrams { get; set; }

    /// <summary>طول به سانتی‌متر</summary>
    public decimal? LengthCm { get; set; }

    /// <summary>عرض به سانتی‌متر</summary>
    public decimal? WidthCm { get; set; }

    /// <summary>ارتفاع به سانتی‌متر</summary>
    public decimal? HeightCm { get; set; }

    public string? Brand { get; set; }

    public decimal BasePrice { get; set; }

    public bool IsDiscountEnabled { get; set; }

    /// <summary>قیمت بعد از تخفیف (در صورت فعال بودن تخفیف)</summary>
    public decimal? DiscountPrice { get; set; }

    public int StockQuantity { get; set; }

    public bool HasSpecialPackaging { get; set; }

    /// <summary>محصول خاص (فیلتر «خاص» در لیست)</summary>
    public bool IsSpecial { get; set; }

    /// <summary>تعداد فروش برای فیلتر پرفروش</summary>
    public int SoldCount { get; set; }

    public ProductStatus Status { get; set; } = ProductStatus.Draft;

    /// <summary>
    /// شناسه فایل‌های آپلودشده از API عمومی فایل (ترتیب = اولویت نمایش، اولین = تصویر اصلی).
    /// جدول جداگانه‌ای برای عکس محصول وجود ندارد.
    /// </summary>
    public List<Guid> ImageFileIds { get; set; } = new();

    public List<ProductColor> Colors { get; set; } = new();
}
