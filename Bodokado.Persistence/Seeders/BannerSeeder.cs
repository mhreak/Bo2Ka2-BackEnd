// Persistence/Seeders/BannerSeeder.cs
using Bodokado.Domain.Entities.Banners;
using Bodokado.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Bodokado.Persistence.Seeders;

public static class BannerSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken ct = default)
    {
        var any = await context.Banners.AnyAsync(b => !b.IsDeleted, ct);
        if (any)
            return;

        var now = DateTime.UtcNow;

        var banners = new List<Banner>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "ارسال رایگان هدیه",
                Description = "تا پایان هفته با خرید بالای ۵۰۰ هزار تومان",
                ImageId = null, // بعداً از پنل ادمین تصویر بگذار
                Link = "bodokado://products?filter=special",
                ShowOrder = 1,
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "مجموعه‌های لانچ",
                Description = "جدیدترین هدایا برای مناسبت‌های خاص",
                ImageId = null,
                Link = "bodokado://categories",
                ShowOrder = 2,
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "هدیه سازمانی",
                Description = "پکیج‌های ویژه شرکت‌ها و سازمان‌ها",
                ImageId = null,
                Link = "bodokado://corporate",
                ShowOrder = 3,
                IsActive = true,
                CreatedAt = now
            }
        };

        context.Banners.AddRange(banners);
        await context.SaveChangesAsync(ct);
    }
}