using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.CustomerModule.Products.DTOs;

/// <summary>
/// کوئری فیلتر لیست محصولات برای اپ مشتری
/// </summary>
public class CustomerProductListQuery : PaginationQuery
{
    /// <summary>جستجو بر اساس نام محصول / برند</summary>
    public string? Search { get; set; }

    /// <summary>محصولات یک فروشگاه خاص (صفحه فروشگاه)</summary>
    public Guid? ShopId { get; set; }

    /// <summary>محصولات فروشگاه‌های یک دسته‌بندی خاص</summary>
    public Guid? ShopCategoryId { get; set; }

    /// <summary>فیلتر بر اساس برند</summary>
    public string? Brand { get; set; }

    /// <summary>حداقل قیمت مؤثر (بعد از تخفیف)</summary>
    public decimal? MinPrice { get; set; }

    /// <summary>حداکثر قیمت مؤثر (بعد از تخفیف)</summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>فقط محصولات ویژه</summary>
    public bool? IsSpecial { get; set; }

    /// <summary>فقط محصولات تخفیف‌دار</summary>
    public bool? HasDiscount { get; set; }

    /// <summary>فقط محصولات موجود در انبار</summary>
    public bool? InStockOnly { get; set; }

    /// <summary>نوع محصول</summary>
    public ProductType? ProductType { get; set; }

    /// <summary>ترتیب نمایش</summary>
    public ProductSortBy SortBy { get; set; } = ProductSortBy.Newest;
}
