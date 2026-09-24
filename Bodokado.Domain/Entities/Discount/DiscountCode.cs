// Domain/Entities/Discount/DiscountCode.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Entities.Order;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Enums;

namespace Bodokado.Domain.Entities.Discounts;

public class DiscountCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public DiscountCodeType DiscountCodeType { get; set; }
    public DiscountType DiscountType { get; set; }

    /// <summary>درصد یا مبلغ — بسته به DiscountType</summary>
    public int? Discount { get; set; }

    public string? IncludedOrganizationIds { get; set; }
    public string? ExcludedOrganizationIds { get; set; }
    public string? IncludedShopIds { get; set; }
    public string? ExcludedShopIds { get; set; }
    public string? IncludedProductCategoryIds { get; set; }
    public string? ExcludedProductCategoryIds { get; set; }
    public string? IncludedProductIds { get; set; }
    public string? ExcludedProductIds { get; set; }

    public DateTime? StartDateTime { get; set; }
    public DateTime? FinishDateTime { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public Guid? OrderCustomizationTypeId { get; set; }
    public OrderCustomizationType? OrderCustomizationType { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsActiveByAdmin { get; set; } = true;
}