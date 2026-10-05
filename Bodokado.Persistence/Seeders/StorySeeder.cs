// Persistence/Seeders/StorySeeder.cs
using Bodokado.Domain.Entities;
using Bodokado.Domain.Entities.Stories;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Bodokado.Persistence.Seeders;

public static class StorySeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        IHostEnvironment env,
        IConfiguration config,
        CancellationToken ct = default)
    {
        if (await context.Stories.AnyAsync(s => !s.IsDeleted, ct))
            return;

        var storageRoot = config["Storage:RootPath"];
        if (string.IsNullOrWhiteSpace(storageRoot))
            storageRoot = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "..", "uploads"));
        storageRoot = Path.GetFullPath(storageRoot);

        var seedSourceDir = Path.Combine(env.ContentRootPath, "SeedData", "Stories");
        var relativeFolder = Path.Combine("Seed", "Stories");
        var targetFolder = Path.Combine(storageRoot, relativeFolder);
        Directory.CreateDirectory(targetFolder);

        var now = DateTime.UtcNow;
        var uploaderId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // اگر فایل نبود، MediaFileId = null می‌ماند (مثل قبل)
        Guid? CopySeedImage(string fileName)
        {
            var sourcePath = Path.Combine(seedSourceDir, fileName);
            if (!File.Exists(sourcePath))
                return null;

            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(ext))
                ext = ".jpg";

            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var physicalPath = Path.Combine(targetFolder, uniqueName);
            File.Copy(sourcePath, physicalPath, overwrite: true);

            var fileInfo = new FileInfo(physicalPath);
            var asset = new FileAsset
            {
                Id = Guid.NewGuid(),
                FileName = uniqueName,
                Extension = ext,
                Size = fileInfo.Length,
                UploadFileType = GuessContentType(ext),
                Path = Path.Combine("uploads", relativeFolder, uniqueName).Replace("\\", "/"),
                UploaderId = uploaderId,
                OwnerId = uploaderId,
                FileType = UploadFileType.Post,
                CreatedAt = now,
                IsDeleted = false
            };
            context.Set<FileAsset>().Add(asset);
            return asset.Id;
        }

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
                MediaFileId = CopySeedImage("story1.jpg"),
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
                MediaFileId = CopySeedImage("story2.jpg"),
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
                MediaFileId = CopySeedImage("story3.jpg"),
                StoryButtonText = null,
                StoryButtonClickEntityId = null,
                StoryButtonClickActionType = StoryButtonClickActionType.None,
                ShowOrder = 3,
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
                MediaFileId = CopySeedImage("story-middle.jpg"),
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

    private static string GuessContentType(string ext) => ext.ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".webp" => "image/webp",
        ".gif" => "image/gif",
        _ => "image/jpeg"
    };
}