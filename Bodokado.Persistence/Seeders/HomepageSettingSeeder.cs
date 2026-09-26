// Persistence/Seeders/HomepageSettingSeeder.cs
using System.Text.Json;
using Bodokado.Domain.Entities.Settings;
using Bodokado.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Bodokado.Persistence.Seeders;

public static class HomepageSettingSeeder
{
    public const string HomepageKey = "homepage";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static async Task SeedAsync(AppDbContext context, CancellationToken ct = default)
    {
        var exists = await context.Settings
            .AnyAsync(x => x.Key == HomepageKey && !x.IsDeleted, ct);

        if (exists)
            return;

        // ترتیب مطابق UI: استوری → بنر → دسته‌بندی → پرفروش → تازه‌ها → یادگاری → فروشگاه‌های معتبر
        // سرچ و هدر ثابت در setting ذخیره نمی‌شوند
        var layout = new
        {
            sections = new object[]
            {
                new
                {
                    type = "stories",
                    order = 1,
                    isVisible = true,
                    title = (string?)null
                },
                new
                {
                    type = "banner",
                    order = 2,
                    isVisible = true,
                    title = (string?)null
                },
                new
                {
                    type = "categories",
                    order = 3,
                    isVisible = true,
                    title = "دسته‌بندی‌های لانچ"
                },
                new
                {
                    type = "bestSellers",
                    order = 4,
                    isVisible = true,
                    title = "پرفروش‌های لانچ",
                    style = new
                    {
                        backgroundImage = (string?)null,   // URL یا fileId — فرانت پر می‌کند
                        backgroundColor = "#F5F0FF",
                        pattern = "dots",                   // none | dots | grid | waves
                        gradient = new
                        {
                            from = "#E8DEFF",
                            to = "#FFFFFF",
                            angle = 180
                        }
                    }
                },
                new
                {
                    type = "newest",
                    order = 5,
                    isVisible = true,
                    title = "تازه‌ترین‌ها",
                    style = new
                    {
                        backgroundImage = (string?)null,
                        backgroundColor = "#FFFFFF",
                        pattern = "none",
                        gradient = (object?)null
                    }
                },
                new
                {
                    type = "souvenirs",
                    order = 6,
                    isVisible = true,
                    title = "یادگاری‌ها",
                    style = new
                    {
                        backgroundImage = (string?)null,
                        backgroundColor = "#FFFFFF",
                        pattern = "none",
                        gradient = (object?)null
                    }
                },
                new
                {
                    type = "trustedShops",
                    order = 7,
                    isVisible = true,
                    title = "فروشگاه‌های معتبر",
                    style = new
                    {
                        backgroundImage = (string?)null,
                        backgroundColor = "#FFFFFF",
                        pattern = "none",
                        gradient = (object?)null
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(layout, JsonOptions);

        context.Settings.Add(new Setting
        {
            Id = Guid.NewGuid(),
            Key = HomepageKey,
            Value = json,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync(ct);
    }
}