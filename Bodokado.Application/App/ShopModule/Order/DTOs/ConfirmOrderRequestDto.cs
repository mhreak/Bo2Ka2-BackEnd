namespace Bodokado.Application.App.ShopModule.Orders.DTOs;

public class ConfirmOrderRequestDto
{
    public string? PackagingType { get; set; }
    public string? PackagingNote { get; set; }
    public bool? HasSpecialPackaging { get; set; }
    public decimal? PackagingCost { get; set; }
    public decimal? ShippingCost { get; set; }
    public bool? ApplyDiscountCode { get; set; }
    public string? ShopNote { get; set; }
}
