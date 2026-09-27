// Domain/Entities/Banners/Banner.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Entities;

namespace Bodokado.Domain.Entities.Banners;

public class Banner : BaseEntity
{
    public string? Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid? ImageId { get; set; }
    public FileAsset? Image { get; set; }

    /// <summary>لینک کلیک بنر (مثلاً deep link یا URL)</summary>
    public string? Link { get; set; }

    public short ShowOrder { get; set; }
    public bool IsActive { get; set; } = true;
}