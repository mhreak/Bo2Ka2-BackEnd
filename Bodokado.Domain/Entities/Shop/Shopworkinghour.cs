using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities.Shops;

public class ShopWorkingHour : BaseEntity
{
    public Guid ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public bool IsClosed { get; set; }
}