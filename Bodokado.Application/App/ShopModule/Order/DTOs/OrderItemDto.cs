namespace Bodokado.Application.App.ShopModule.Orders.DTOs;

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid? ProductImageFileId { get; set; }
    public string? ProductImagePath { get; set; }
    public string? SelectedColor { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
