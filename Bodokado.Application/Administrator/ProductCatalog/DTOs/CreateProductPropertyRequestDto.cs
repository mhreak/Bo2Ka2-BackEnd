using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;

public class CreateProductPropertyRequestDto
{
    public string Name { get; set; } = string.Empty;
    public ProductPropertyType Type { get; set; } = ProductPropertyType.Text;
    public int SortOrder { get; set; }
    public Guid? ProductCategoryId { get; set; }
}