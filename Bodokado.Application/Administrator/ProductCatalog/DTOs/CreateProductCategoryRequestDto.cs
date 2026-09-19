namespace Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;

public class CreateProductCategoryRequestDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid? ParentCategoryId { get; set; }
    public Guid? ImageId { get; set; }
}