// Domain/Entities/Story/Story.cs
using Bodokado.Domain.Common;
using Bodokado.Domain.Entities;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Enums;

namespace Bodokado.Domain.Entities.Stories;

public class Story : BaseEntity
{
    /// <summary>اختیاری — استوری ادمین می‌تواند بدون فروشگاه باشد</summary>
    public Guid? ShopId { get; set; }
    public Shop? Shop { get; set; }

    public bool IsPublished { get; set; }
    public DateTime? PublishDateTime { get; set; }

    /// <summary>غیرفعال‌سازی توسط ادمین</summary>
    public bool DisabledByAdmin { get; set; }

    /// <summary>فایل تصویر یا ویدیو استوری</summary>
    public Guid? MediaFileId { get; set; }
    public FileAsset? MediaFile { get; set; }

    public string? StoryButtonText { get; set; }
    public Guid? StoryButtonClickEntityId { get; set; }
    public StoryButtonClickActionType StoryButtonClickActionType { get; set; }
        = StoryButtonClickActionType.None;

    public short ShowOrder { get; set; }
    public StoryShowPlace ShowPlace { get; set; }

    public bool IsActiveByAdmin { get; set; } = true;
}