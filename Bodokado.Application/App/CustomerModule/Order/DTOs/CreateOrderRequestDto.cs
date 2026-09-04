using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.CustomerModule.Orders.DTOs;

public class CreateOrderRequestDto
{
    /// <summary>همه آیتم‌ها باید متعلق به یک فروشگاه باشند</summary>
    public List<CreateOrderItemRequestDto> Items { get; set; } = new();

    public string BuyerName { get; set; } = string.Empty;
    public string BuyerPhone { get; set; } = string.Empty;
    public string? DeliveryAddress { get; set; }
    public Guid? ProvinceId { get; set; }
    public Guid? CityId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public ShippingMethod ShippingMethod { get; set; } = ShippingMethod.Normal;
    public DateTime? DeliveryDate { get; set; }
    public string? DeliveryTimeSlot { get; set; }

    public string? PackagingType { get; set; }
    public string? PackagingNote { get; set; }
    public bool HasSpecialPackaging { get; set; }

    public string? GiftCardType { get; set; }
    public string? GiftCardColor { get; set; }
    public string? RibbonStyle { get; set; }
    public string? GiftCardDesignKey { get; set; }
    public string? GiftMessage { get; set; }
    public string? RecipientName { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Online;
    public string? DiscountCode { get; set; }
}
