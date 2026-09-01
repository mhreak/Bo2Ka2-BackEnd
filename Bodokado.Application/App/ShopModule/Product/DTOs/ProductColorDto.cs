namespace Bodokado.Application.App.ShopModule.Products.DTOs;

public class ProductColorDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? HexCode { get; set; }
    public int SortOrder { get; set; }
}
