using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Shops;

public class ShopCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? IconKey { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public List<Shop> Shops { get; set; } = new();
}