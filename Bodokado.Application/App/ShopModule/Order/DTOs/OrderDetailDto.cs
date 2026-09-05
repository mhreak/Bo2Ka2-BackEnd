using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Orders.DTOs;

public class OrderDetailDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();

    public ShippingMethod ShippingMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? DeliveryTimeSlot { get; set; }

    public string? PackagingType { get; set; }
    public string? PackagingNote { get; set; }
    public bool HasSpecialPackaging { get; set; }

    public string BuyerName { get; set; } = string.Empty;
    public string BuyerPhone { get; set; } = string.Empty;
    public string? DeliveryAddress { get; set; }
    public Guid? CityId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public string? GiftCardType { get; set; }
    public string? GiftCardColor { get; set; }
    public string? RibbonStyle { get; set; }
    public string? GiftCardDesignKey { get; set; }
    public string? GiftMessage { get; set; }
    public string? RecipientName { get; set; }

    public decimal GoodsAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal PackagingCost { get; set; }
    public decimal DiscountAmount { get; set; }
    public bool ApplyDiscountCode { get; set; }
    public decimal FinalAmount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    public string? RejectionReason { get; set; }
    public string? ShopNote { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
}
