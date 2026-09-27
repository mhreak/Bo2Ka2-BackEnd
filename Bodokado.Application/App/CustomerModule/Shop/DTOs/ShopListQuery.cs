using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.CustomerModule.Shops.DTOs;

/// <summary>
/// کوئری فیلتر لیست فروشگاه‌ها برای اپ مشتری (صفحه اصلی / جستجو)
/// </summary>
public class ShopListQuery : PaginationQuery
{
    /// <summary>جستجو بر اساس نام فروشگاه / آدرس</summary>
    public string? Search { get; set; }

    /// <summary>فیلتر بر اساس دسته‌بندی فروشگاه (ردیف آیکون‌های بالای صفحه)</summary>
    public Guid? ShopCategoryId { get; set; }

    /// <summary>فیلتر بر اساس شهر</summary>
    public Guid? CityId { get; set; }

    /// <summary>فیلتر بر اساس استان (از طریق شهر)</summary>
    public Guid? ProvinceId { get; set; }

    /// <summary>فقط فروشگاه‌هایی که هم‌اکنون باز هستند</summary>
    public bool? OnlyOpenNow { get; set; }

    /// <summary>فقط فروشگاه‌هایی که استوری فعال دارند</summary>
    public bool? HasStories { get; set; }

    /// <summary>فقط فروشگاه‌هایی که حداقل یک محصول ویژه (تخفیف‌دار/اسپشیال) دارند</summary>
    public bool? HasSpecialProducts { get; set; }

    /// <summary>فقط فروشگاه‌های تازه معرفی‌شده (N روز اخیر) - «فروشگاه‌های تازه معرفی‌شده»</summary>
    public bool? OnlyNew { get; set; }

    /// <summary>ترتیب نمایش</summary>
    public ShopSortBy SortBy { get; set; } = ShopSortBy.Newest;
}
