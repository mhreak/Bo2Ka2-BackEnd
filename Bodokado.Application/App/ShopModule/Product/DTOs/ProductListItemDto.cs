using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Products.DTOs;

public class ProductListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public bool IsDiscountEnabled { get; set; }
    public decimal? DiscountPrice { get; set; }
    public decimal EffectivePrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsInStock { get; set; }
    public bool IsSpecial { get; set; }
    public int SoldCount { get; set; }
    public ProductStatus Status { get; set; }
    public string? PrimaryImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
}
