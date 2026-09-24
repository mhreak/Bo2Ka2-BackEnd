// ProductProperty.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Enums;

namespace Bodokado.Domain.Entities.Products;

public class ProductAttribute : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ProductAttributeType Type { get; set; } = ProductAttributeType.Text;

    /// <summary>ترتیب نمایش (OrderItemsBy)</summary>
    public int SortOrder { get; set; }

    public Guid? ProductCategoryId { get; set; }
    public ProductCategory? ProductCategory { get; set; }

    public bool UseForProductVariants { get; set; }

    public List<ProductAttributeValue> Values { get; set; } = new();
    public List<ProductProductAttribute> ProductAttributeLinks { get; set; } = new();
}