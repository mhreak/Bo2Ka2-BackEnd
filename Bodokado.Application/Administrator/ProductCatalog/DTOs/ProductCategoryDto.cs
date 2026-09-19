namespace Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;

public class ProductCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public Guid? ImageId { get; set; }
    public string? ImagePath { get; set; }
    public List<ProductCategoryDto> Children { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}