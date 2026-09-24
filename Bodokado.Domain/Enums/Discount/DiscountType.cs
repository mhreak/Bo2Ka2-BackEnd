// Domain/Enums/Discount/DiscountType.cs
namespace Bodokado.Domain.Enums;

public enum DiscountType : short
{
    PercentageDiscount = 1,
    FixedAmountDiscount = 2,
    FreeDelivery = 3,
    FreeOrderOption = 4
}