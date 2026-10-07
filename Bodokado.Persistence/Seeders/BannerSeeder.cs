using Bodokado.Domain.Entities.Banners;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Bodokado.Persistence.Seeders;

public static class BannerSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken ct = default)
    {
        var existing = await context.Banners.Where(b => !b.IsDeleted).ToListAsync(ct);
        if (existing.Count > 0)
        {
            var updated = false;
            foreach (var b in existing)
            {
                if (b.ShowPlace != BannerShowPlace.HomePage)
                {
                    b.ShowPlace = BannerShowPlace.HomePage;
                    updated = true;
                }
            }
            if (updated)
                await context.SaveChangesAsync(ct);

            return;
        }

        var now = DateTime.UtcNow;

        var banners = new List<Banner>
        {
            new()
            {
                Id = DeterministicGuid.Create("SeedBanner_1"),
                Title = "ارسال رایگان هدیه",
                Description = "تا پایان هفته با خرید بالای ۵۰۰ هزار تومان",
                ImageId = null, // بعداً از پنل ادمین تصویر بگذار
                Link = "bodokado://products?filter=special",
                ShowOrder = 1,
                IsActive = true,
                ShowPlace = BannerShowPlace.HomePage,
                CreatedAt = now
            },
            new()
            {
                Id = DeterministicGuid.Create("SeedBanner_2"),
                Title = "مجموعه‌های لانچ",
                Description = "جدیدترین هدایا برای مناسبت‌های خاص",
                ImageId = null,
                Link = "bodokado://categories",
                ShowOrder = 2,
                IsActive = true,
                ShowPlace = BannerShowPlace.HomePage,
                CreatedAt = now
            },
            new()
            {
                Id = DeterministicGuid.Create("SeedBanner_3"),
                Title = "هدیه سازمانی",
                Description = "پکیج‌های ویژه شرکت‌ها و سازمان‌ها",
                ImageId = null,
                Link = "bodokado://user-organization",
                ShowOrder = 3,
                IsActive = true,
                ShowPlace = BannerShowPlace.HomePage,
                CreatedAt = now
            }
        };

        context.Banners.AddRange(banners);
        await context.SaveChangesAsync(ct);
    }
}