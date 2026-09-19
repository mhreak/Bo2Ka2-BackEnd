using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Products;

/// <summary>مقدار ویژگی اختصاص‌یافته به یک محصول</summary>
public class ProductProductProperty : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid? ProductPropertyId { get; set; }
    public ProductProperty? ProductProperty { get; set; }

    /// <summary>مقدار متنی ویژگی برای این محصول (حداکثر ۱۰۰ کاراکتر)</summary>
    public string? Value { get; set; }
}