using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;

public class ProductPropertyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ProductPropertyType Type { get; set; }
    public int SortOrder { get; set; }
    public Guid? ProductCategoryId { get; set; }
    public string? ProductCategoryName { get; set; }
    public DateTime CreatedAt { get; set; }
}