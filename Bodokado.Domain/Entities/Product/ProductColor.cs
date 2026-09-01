using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Products;

public class ProductColor : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    /// <summary>کد رنگ اختیاری (مثلاً #F5F0E6)</summary>
    public string? HexCode { get; set; }

    public int SortOrder { get; set; }
}
