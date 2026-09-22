using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Products;

/// <summary>مقادیر از پیش‌تعریف‌شده برای ویژگی از نوع انتخابی</summary>
public class ProductAttributeValue : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public Guid ProductAttributeId { get; set; }
    public ProductAttribute ProductAttribute { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    /// <summary>ترتیب نمایش</summary>
    public int SortOrder { get; set; }
}