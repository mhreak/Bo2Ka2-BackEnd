// Persistence/Seeders/StorySeeder.cs
using Bodokado.Domain.Entities.Stories;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Bodokado.Persistence.Seeders;

public static class StorySeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken ct = default)
    {
        var any = await context.Stories.AnyAsync(s => !s.IsDeleted, ct);
        if (any)
            return;

        var now = DateTime.UtcNow;

        var stories = new List<Story>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ShopId = null,
                IsPublished = true,
                PublishDateTime = now,
                DisabledByAdmin = false,
                IsActiveByAdmin = true,
                MediaFileId = null, // بعداً از پنل ادمین فایل بگذار
                StoryButtonText = null,
                StoryButtonClickEntityId = null,
                StoryButtonClickActionType = StoryButtonClickActionType.None,
                ShowOrder = 1,
                ShowPlace = StoryShowPlace.ApplicationHomePageTopStorySection,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                ShopId = null,
                IsPublished = true,
                PublishDateTime = now,
                DisabledByAdmin = false,
                IsActiveByAdmin = true,
                MediaFileId = null,
                StoryButtonText = "مشاهده",
                StoryButtonClickEntityId = null,
                StoryButtonClickActionType = StoryButtonClickActionType.None,
                ShowOrder = 2,
                ShowPlace = StoryShowPlace.ApplicationHomePageTopStorySection,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                ShopId = null,
                IsPublished = true,
                PublishDateTime = now,
                DisabledByAdmin = false,
                IsActiveByAdmin = true,
                MediaFileId = null,
                StoryButtonText = null,
                StoryButtonClickEntityId = null,
                StoryButtonClickActionType = StoryButtonClickActionType.None,
                ShowOrder = 3,
                ShowPlace = StoryShowPlace.ApplicationHomePageTopStorySection,
                CreatedAt = now
            },
            // استوری میانی homepage (اختیاری)
            new()
            {
                Id = Guid.NewGuid(),
                ShopId = null,
                IsPublished = true,
                PublishDateTime = now,
                DisabledByAdmin = false,
                IsActiveByAdmin = true,
                MediaFileId = null,
                StoryButtonText = null,
                StoryButtonClickEntityId = null,
                StoryButtonClickActionType = StoryButtonClickActionType.None,
                ShowOrder = 1,
                ShowPlace = StoryShowPlace.ApplicationHomePageMiddleStorySection,
                CreatedAt = now
            }
        };

        context.Stories.AddRange(stories);
        await context.SaveChangesAsync(ct);
    }
}