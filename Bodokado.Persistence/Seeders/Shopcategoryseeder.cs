using Bodokado.Domain.Entities.Shops;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Seeders;

public static class ShopCategorySeeder
{
    private static readonly string[] Categories =
    {
        "موبایل و کامپیوتر",
        "جواهرات",
        "عطر و ادکلن",
        "پوشاک",
        "کیف و کفش",
        "لوازم آرایشی و بهداشتی",
        "لوازم خانگی",
        "کتاب و لوازم تحریر",
        "اسباب‌بازی",
        "خوراکی و شیرینی",
        "ورزش و سرگرمی",
        "سایر"
    };

    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.ShopCategories.Any())
            return;

        var sortOrder = 0;
        foreach (var name in Categories)
        {
            sortOrder++;
            var id = DeterministicGuid.Create($"ShopCategory_{name}");
            context.ShopCategories.Add(new ShopCategory
            {
                Id = id,
                Name = name,
                SortOrder = sortOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }
}