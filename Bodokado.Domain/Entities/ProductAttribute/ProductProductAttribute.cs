using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Products;

/// <summary>مقدار ویژگی اختصاص‌یافته به یک محصول</summary>
public class ProductProductAttribute : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid? ProductAttributeId { get; set; }
    public ProductAttribute? ProductAttribute { get; set; }

    /// <summary>مقدار متنی ویژگی برای این محصول (حداکثر ۱۰۰ کاراکتر)</summary>
    public string? Value { get; set; }
}