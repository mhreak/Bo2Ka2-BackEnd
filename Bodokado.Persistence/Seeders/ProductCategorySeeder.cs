// Persistence/Seeders/ProductCategorySeeder.cs
using Bodokado.Domain.Entities.Products;
using Bodokado.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Bodokado.Persistence.Seeders;

public static class ProductCategorySeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken ct = default)
    {
        var any = await context.ProductCategories.AnyAsync(c => !c.IsDeleted, ct);
        if (any)
            return;

        var now = DateTime.UtcNow;

        // ریشه — دسته‌بندی‌های اصلی (مطابق UI لانچ)
        var roots = new List<ProductCategory>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "هدیه روز تولد",
                IsActive = true,
                ParentCategoryId = null,
                ImageId = null,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "هدیه سالگرد",
                IsActive = true,
                ParentCategoryId = null,
                ImageId = null,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "هدیه عاشقانه",
                IsActive = true,
                ParentCategoryId = null,
                ImageId = null,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "هدیه سازمانی",
                IsActive = true,
                ParentCategoryId = null,
                ImageId = null,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "یادگاری و دکوری",
                IsActive = true,
                ParentCategoryId = null,
                ImageId = null,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "گل و گیاه",
                IsActive = true,
                ParentCategoryId = null,
                ImageId = null,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "شیرینی و شکلات",
                IsActive = true,
                ParentCategoryId = null,
                ImageId = null,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "لوازم دیجیتال",
                IsActive = true,
                ParentCategoryId = null,
                ImageId = null,
                CreatedAt = now
            }
        };

        context.ProductCategories.AddRange(roots);
        await context.SaveChangesAsync(ct);

        // زیر‌دسته‌های نمونه برای یکی از ریشه‌ها
        var birthdayId = roots[0].Id;
        var children = new List<ProductCategory>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "هدیه آقایان",
                IsActive = true,
                ParentCategoryId = birthdayId,
                ImageId = null,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "هدیه خانم‌ها",
                IsActive = true,
                ParentCategoryId = birthdayId,
                ImageId = null,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "هدیه کودک",
                IsActive = true,
                ParentCategoryId = birthdayId,
                ImageId = null,
                CreatedAt = now
            }
        };

        context.ProductCategories.AddRange(children);
        await context.SaveChangesAsync(ct);
    }
}