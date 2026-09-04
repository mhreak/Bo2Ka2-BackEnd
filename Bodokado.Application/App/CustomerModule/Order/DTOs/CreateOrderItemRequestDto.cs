namespace Bodokado.Application.App.CustomerModule.Orders.DTOs;

public class CreateOrderItemRequestDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public string? SelectedColor { get; set; }
}
