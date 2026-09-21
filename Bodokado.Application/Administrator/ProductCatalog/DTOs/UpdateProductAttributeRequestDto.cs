using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;

public class UpdateProductAttributeRequestDto
{
    public string Name { get; set; } = string.Empty;
    public ProductAttributeType Type { get; set; } = ProductAttributeType.Text;
    public int SortOrder { get; set; }
    public Guid? ProductCategoryId { get; set; }
}