// Domain/Entities/Rating/EntityRating.cs
using Bodokado.Domain.Entities.ShopRatingOptions;
using Bodokado.Domain.Entities.Shops;

namespace Bodokado.Domain.Entities.Ratings;

public class EntityRating
{
    /// <summary>شناسه موجودیت امتیازدهنده (مثلاً Shop یا Product)</summary>
    public Guid EntityId { get; set; }

    public Guid RatingOptionId { get; set; }
    public ShopRatingOption RatingOption { get; set; } = null!;

    public double Score { get; set; }

    public DateTime InsertDateTime { get; set; } = DateTime.UtcNow;
}