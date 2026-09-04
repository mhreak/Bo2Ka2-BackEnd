using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Orders.DTOs;

public class OrderListItemDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImagePath { get; set; }
    public decimal FinalAmount { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public int ItemsCount { get; set; }
}
