// Application/.../Stories/DTOs/StoryDtos.cs
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.AdminModule.Stories.DTOs;

public class StoryDto
{
    public Guid Id { get; set; }
    public Guid? ShopId { get; set; }
    public string? ShopName { get; set; }

    public bool IsPublished { get; set; }
    public DateTime? PublishDateTime { get; set; }
    public bool DisabledByAdmin { get; set; }
    public bool IsActiveByAdmin { get; set; }

    public Guid? MediaFileId { get; set; }
    public string? MediaPath { get; set; }

    public string? MediaFileName { get; set; }
    public string? MediaExtension { get; set; }
    public string? MediaContentType { get; set; } // image/... یا video/...
    public string? MediaUploadFileType { get; set; }

    public string? StoryButtonText { get; set; }
    public Guid? StoryButtonClickEntityId { get; set; }
    public StoryButtonClickActionType StoryButtonClickActionType { get; set; }

    public short ShowOrder { get; set; }
    public StoryShowPlace ShowPlace { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class CreateStoryRequestDto
{
    public Guid? ShopId { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishDateTime { get; set; }

    public Guid? MediaFileId { get; set; }

    public string? StoryButtonText { get; set; }
    public Guid? StoryButtonClickEntityId { get; set; }
    public StoryButtonClickActionType StoryButtonClickActionType { get; set; }
        = StoryButtonClickActionType.None;

    public short ShowOrder { get; set; }
    public StoryShowPlace ShowPlace { get; set; }
}

public class UpdateStoryRequestDto : CreateStoryRequestDto
{
    /// <summary>فقط ادمین</summary>
    public bool? DisabledByAdmin { get; set; }
    public bool? IsActiveByAdmin { get; set; }
}

public class StoryListQuery
{
    public StoryShowPlace? ShowPlace { get; set; }
    public Guid? ShopId { get; set; }
    public bool? IsPublished { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}