// ProductCategory.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Entities;

namespace Bodokado.Domain.Entities.Products;

public class ProductCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Guid? ParentCategoryId { get; set; }
    public ProductCategory? ParentCategory { get; set; }
    public List<ProductCategory> Children { get; set; } = new();

    public Guid? ImageId { get; set; }
    public FileAsset? Image { get; set; }

    public List<ProductAttribute> Attributes { get; set; } = new();
}