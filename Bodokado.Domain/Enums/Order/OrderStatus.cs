namespace Bodokado.Domain.Enums;

public enum OrderStatus
{
    /// <summary>در انتظار تایید فروشگاه</summary>
    Pending = 0,
    /// <summary>تایید شده</summary>
    Confirmed = 1,
    /// <summary>رد شده</summary>
    Rejected = 2,
    /// <summary>ارسال شده</summary>
    Shipped = 3,
    /// <summary>تحویل شده</summary>
    Delivered = 4,
    /// <summary>لغو شده</summary>
    Cancelled = 5
}
