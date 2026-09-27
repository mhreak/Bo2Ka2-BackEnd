// Persistence/Seeders/ProductAttributeSeeder.cs
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Bodokado.Persistence.Seeders;

public static class ProductAttributeSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken ct = default)
    {
        var any = await context.ProductAttributes.AnyAsync(a => !a.IsDeleted, ct);
        if (any)
            return;

        var now = DateTime.UtcNow;

        var digitalCategoryId = await context.ProductCategories
            .Where(c => !c.IsDeleted && c.Name == "لوازم دیجیتال")
            .Select(c => (Guid?)c.Id)
            .FirstOrDefaultAsync(ct);

        var flowerCategoryId = await context.ProductCategories
            .Where(c => !c.IsDeleted && c.Name == "گل و گیاه")
            .Select(c => (Guid?)c.Id)
            .FirstOrDefaultAsync(ct);

        var sweetsCategoryId = await context.ProductCategories
            .Where(c => !c.IsDeleted && c.Name == "شیرینی و شکلات")
            .Select(c => (Guid?)c.Id)
            .FirstOrDefaultAsync(ct);

        // ویژگی‌های عمومی (بدون دسته‌بندی خاص - قابل استفاده روی همه محصولات)
        var colorAttribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Name = "رنگ",
            Type = ProductAttributeType.Selectable,
            SortOrder = 1,
            ProductCategoryId = null,
            UseForProductVariants = true,
            CreatedAt = now
        };

        var sizeAttribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Name = "سایز",
            Type = ProductAttributeType.Selectable,
            SortOrder = 2,
            ProductCategoryId = null,
            UseForProductVariants = true,
            CreatedAt = now
        };

        var giftWrapAttribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Name = "بسته‌بندی هدیه",
            Type = ProductAttributeType.Selectable,
            SortOrder = 3,
            ProductCategoryId = null,
            UseForProductVariants = false,
            CreatedAt = now
        };

        var materialAttribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Name = "جنس",
            Type = ProductAttributeType.Text,
            SortOrder = 4,
            ProductCategoryId = null,
            UseForProductVariants = false,
            CreatedAt = now
        };

        // ویژگی‌های اختصاصی دسته «لوازم دیجیتال»
        var warrantyAttribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Name = "گارانتی",
            Type = ProductAttributeType.Selectable,
            SortOrder = 1,
            ProductCategoryId = digitalCategoryId,
            UseForProductVariants = false,
            CreatedAt = now
        };

        var storageAttribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Name = "حافظه",
            Type = ProductAttributeType.Selectable,
            SortOrder = 2,
            ProductCategoryId = digitalCategoryId,
            UseForProductVariants = true,
            CreatedAt = now
        };

        // ویژگی اختصاصی دسته «گل و گیاه»
        var flowerTypeAttribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Name = "نوع گل",
            Type = ProductAttributeType.Selectable,
            SortOrder = 1,
            ProductCategoryId = flowerCategoryId,
            UseForProductVariants = false,
            CreatedAt = now
        };

        // ویژگی اختصاصی دسته «شیرینی و شکلات»
        var flavorAttribute = new ProductAttribute
        {
            Id = Guid.NewGuid(),
            Name = "طعم",
            Type = ProductAttributeType.Selectable,
            SortOrder = 1,
            ProductCategoryId = sweetsCategoryId,
            UseForProductVariants = true,
            CreatedAt = now
        };

        var attributes = new List<ProductAttribute>
        {
            colorAttribute, sizeAttribute, giftWrapAttribute, materialAttribute,
            warrantyAttribute, storageAttribute, flowerTypeAttribute, flavorAttribute
        };

        context.ProductAttributes.AddRange(attributes);
        await context.SaveChangesAsync(ct);

        // مقادیر ویژگی‌ها
        var values = new List<ProductAttributeValue>
        {
            // رنگ
            new() { Id = Guid.NewGuid(), Title = "قرمز", Value = "red", ProductAttributeId = colorAttribute.Id, IsActive = true, SortOrder = 1, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "مشکی", Value = "black", ProductAttributeId = colorAttribute.Id, IsActive = true, SortOrder = 2, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "سفید", Value = "white", ProductAttributeId = colorAttribute.Id, IsActive = true, SortOrder = 3, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "صورتی", Value = "pink", ProductAttributeId = colorAttribute.Id, IsActive = true, SortOrder = 4, CreatedAt = now },

            // سایز
            new() { Id = Guid.NewGuid(), Title = "کوچک", Value = "S", ProductAttributeId = sizeAttribute.Id, IsActive = true, SortOrder = 1, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "متوسط", Value = "M", ProductAttributeId = sizeAttribute.Id, IsActive = true, SortOrder = 2, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "بزرگ", Value = "L", ProductAttributeId = sizeAttribute.Id, IsActive = true, SortOrder = 3, CreatedAt = now },

            // بسته‌بندی هدیه
            new() { Id = Guid.NewGuid(), Title = "بدون بسته‌بندی", Value = "none", ProductAttributeId = giftWrapAttribute.Id, IsActive = true, SortOrder = 1, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "بسته‌بندی ساده", Value = "simple", ProductAttributeId = giftWrapAttribute.Id, IsActive = true, SortOrder = 2, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "بسته‌بندی لوکس", Value = "luxury", ProductAttributeId = giftWrapAttribute.Id, IsActive = true, SortOrder = 3, CreatedAt = now },

            // گارانتی
            new() { Id = Guid.NewGuid(), Title = "بدون گارانتی", Value = "none", ProductAttributeId = warrantyAttribute.Id, IsActive = true, SortOrder = 1, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "۶ ماه", Value = "6m", ProductAttributeId = warrantyAttribute.Id, IsActive = true, SortOrder = 2, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "۱۸ ماه", Value = "18m", ProductAttributeId = warrantyAttribute.Id, IsActive = true, SortOrder = 3, CreatedAt = now },

            // حافظه
            new() { Id = Guid.NewGuid(), Title = "۶۴ گیگابایت", Value = "64GB", ProductAttributeId = storageAttribute.Id, IsActive = true, SortOrder = 1, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "۱۲۸ گیگابایت", Value = "128GB", ProductAttributeId = storageAttribute.Id, IsActive = true, SortOrder = 2, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "۲۵۶ گیگابایت", Value = "256GB", ProductAttributeId = storageAttribute.Id, IsActive = true, SortOrder = 3, CreatedAt = now },

            // نوع گل
            new() { Id = Guid.NewGuid(), Title = "رز", Value = "rose", ProductAttributeId = flowerTypeAttribute.Id, IsActive = true, SortOrder = 1, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "لیلیوم", Value = "lily", ProductAttributeId = flowerTypeAttribute.Id, IsActive = true, SortOrder = 2, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "ارکیده", Value = "orchid", ProductAttributeId = flowerTypeAttribute.Id, IsActive = true, SortOrder = 3, CreatedAt = now },

            // طعم
            new() { Id = Guid.NewGuid(), Title = "شیری", Value = "milk", ProductAttributeId = flavorAttribute.Id, IsActive = true, SortOrder = 1, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "تلخ", Value = "dark", ProductAttributeId = flavorAttribute.Id, IsActive = true, SortOrder = 2, CreatedAt = now },
            new() { Id = Guid.NewGuid(), Title = "فندقی", Value = "hazelnut", ProductAttributeId = flavorAttribute.Id, IsActive = true, SortOrder = 3, CreatedAt = now },
        };

        context.ProductAttributeValues.AddRange(values);
        await context.SaveChangesAsync(ct);
    }
}