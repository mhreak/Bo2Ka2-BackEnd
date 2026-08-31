namespace Bodokado.Application.App.ShopModule.Registration.DTOs;

public class ShopCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconKey { get; set; }
}