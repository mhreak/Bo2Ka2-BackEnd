using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.CustomerModule.Products.DTOs;

public class CustomerProductListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Brand { get; set; }

    public decimal BasePrice { get; set; }
    public bool IsDiscountEnabled { get; set; }
    public decimal? DiscountPrice { get; set; }
    public decimal EffectivePrice { get; set; }
    public int? DiscountPercent { get; set; }

    public bool IsInStock { get; set; }
    public bool IsSpecial { get; set; }
    public int SoldCount { get; set; }

    public string? PrimaryImagePath { get; set; }
    public ProductType ProductType { get; set; }

    public Guid ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
