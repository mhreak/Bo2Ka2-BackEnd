using Bodokado.Domain.Common;
using Bodokado.Domain.Entities.Products;

namespace Bodokado.Domain.Entities.Orders;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>اسنپ‌شات نام محصول در لحظه سفارش</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>اسنپ‌شات تصویر اصلی محصول</summary>
    public Guid? ProductImageFileId { get; set; }

    public string? SelectedColor { get; set; }

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
