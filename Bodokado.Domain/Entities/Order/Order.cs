using Bodokado.Domain.Common;
using Bodokado.Domain.Entities.Locations;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Enums;

namespace Bodokado.Domain.Entities.Orders;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;

    public Guid ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public Guid CustomerUserId { get; set; }
    public User CustomerUser { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // خریدار / تحویل
    public string BuyerName { get; set; } = string.Empty;
    public string BuyerPhone { get; set; } = string.Empty;
    public string? DeliveryAddress { get; set; }
    public Guid? CityId { get; set; }
    public City? City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // ارسال و زمان
    public ShippingMethod ShippingMethod { get; set; } = ShippingMethod.Normal;
    public DateTime? DeliveryDate { get; set; }
    public string? DeliveryTimeSlot { get; set; }

    // بسته‌بندی
    public string? PackagingType { get; set; }
    public string? PackagingNote { get; set; }
    public bool HasSpecialPackaging { get; set; }

    // هدیه / کارت پستال (اختیاری - مطابق UI چک‌اوت)
    public string? GiftCardType { get; set; }
    public string? GiftCardColor { get; set; }
    public string? RibbonStyle { get; set; }
    public string? GiftCardDesignKey { get; set; }
    public string? GiftMessage { get; set; }
    public string? RecipientName { get; set; }

    // مبالغ
    public decimal GoodsAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal PackagingCost { get; set; }
    public decimal DiscountAmount { get; set; }
    public bool ApplyDiscountCode { get; set; }
    public string? DiscountCode { get; set; }
    public decimal FinalAmount { get; set; }

    // پرداخت
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Online;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public string? RejectionReason { get; set; }
    public string? ShopNote { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? ShippedAt { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
