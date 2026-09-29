// Persistence/Seeders/ShopAndProductSeeder.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bodokado.Application.Common.Helpers;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Constants;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Seeders;

/// <summary>
/// چند فروشگاه و محصول نمونه می‌سازد تا API های مشتری
/// (GetAllShops / GetShopById / GetAllProducts / GetProductById) با داده‌ی واقعی قابل تست باشند.
///
/// داده‌ها طوری چیده شده‌اند که همه‌ی فیلترها را پوشش بدهند:
/// - دسته‌بندی فروشگاه، شهر
/// - باز/بسته بودن هم‌اکنون (onlyOpenNow) → دو الگوی ثابت «همیشه باز» و «همیشه بسته»
/// - تازه معرفی‌شده (onlyNew) → بعضی CreatedAt نزدیک و بعضی قدیمی
/// - استوری فعال (hasStories)
/// - محصول ویژه / تخفیف‌دار (hasSpecialProducts, isSpecial, hasDiscount)
/// - موجود / ناموجود (inStockOnly)
/// - پرفروش (sortBy=BestSelling) → SoldCount متفاوت
/// - بیشترین تخفیف (sortBy=MostDiscounted)
///
/// ایمن برای اجرای مکرر: اگر حداقل یک فروشگاه (غیرحذف‌شده) وجود داشته باشد، seed نمی‌کند.
/// </summary>
public static class ShopAndProductSeeder
{
    private const string ShopRole = RoleNames.Shop;

    /// <summary>رمز عبور یکسان همه‌ی کاربران نمونهٔ فروشگاه (برای تست ورود پنل فروشگاه)</summary>
    public const string DemoShopPassword = "ShopDemo123";

    private record WorkingHourSeed(DayOfWeek DayOfWeek, TimeSpan? OpenTime, TimeSpan? CloseTime, bool IsClosed);

    private record ShopSeed(
        string Name,
        Guid ShopCategoryId,
        Guid? CityId,
        List<WorkingHourSeed> WorkingHours,
        bool EnableStories,
        DateTime CreatedAt);

    private record ProductSeed(
        string Name,
        string Brand,
        decimal BasePrice,
        bool IsDiscountEnabled,
        decimal? DiscountPrice,
        int StockQuantity,
        bool IsSpecial,
        int SoldCount,
        ProductType ProductType,
        DateTime CreatedAt);

    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        CancellationToken ct = default)
    {
        if (!await roleManager.RoleExistsAsync(ShopRole))
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = ShopRole });

        for (var demoShopIndex = 1; demoShopIndex <= 6; demoShopIndex++)
        {
            var existingUser = await userManager.FindByNameAsync($"shop_demo_{demoShopIndex}");
            if (existingUser is not null && !await userManager.IsInRoleAsync(existingUser, ShopRole))
                await userManager.AddToRoleAsync(existingUser, ShopRole);
        }

        var alreadySeeded = await context.Shops.AnyAsync(s => !s.IsDeleted, ct);
        if (alreadySeeded)
            return;

        var categories = await context.ShopCategories
            .Where(c => c.IsActive && !c.IsDeleted)
            .ToListAsync(ct);

        if (categories.Count == 0)
            return; // بدون دسته‌بندی، فروشگاه ساخته نمی‌شود (ShopCategoryId اجباری است)

        var cities = await context.Cities.Take(10).ToListAsync(ct);

        Guid? FindCity(string term) =>
            cities.FirstOrDefault(c => c.Name.Contains(term))?.Id ?? cities.FirstOrDefault()?.Id;

        Guid FindCategory(string term) =>
            categories.FirstOrDefault(c => c.Name.Contains(term))?.Id ?? categories.First().Id;

        var now = DateTime.UtcNow;

        var alwaysOpen = BuildWorkingHours(closedAllDay: false);
        var alwaysClosed = BuildWorkingHours(closedAllDay: true);

        var shopSeeds = new List<ShopSeed>
        {
            new("فروشگاه گلرنگ آرایشی", FindCategory("آرایشی"), FindCity("تهران"), alwaysOpen, true, now.AddDays(-2)),
            new("فروشگاه لاکچری بیوتی", FindCategory("آرایشی"), FindCity("اصفهان"), alwaysClosed, false, now.AddDays(-90)),
            new("فروشگاه دیجی موبایل پارس", FindCategory("موبایل"), FindCity("تهران"), alwaysOpen, true, now.AddDays(-10)),
            new("فروشگاه پوشاک آرمانی", FindCategory("پوشاک"), FindCity("شیراز"), alwaysOpen, false, now.AddDays(-120)),
            new("فروشگاه کیف و کفش رویا", FindCategory("کیف"), FindCity("تهران"), alwaysClosed, true, now.AddDays(-1)),
            new("فروشگاه خوراکی شیرین‌عسل", FindCategory("خوراکی"), FindCity("مشهد"), alwaysOpen, false, now.AddDays(-5)),
        };

        var shopIndex = 0;
        foreach (var seed in shopSeeds)
        {
            shopIndex++;

            var userName = $"shop_demo_{shopIndex}";
            var user = await userManager.FindByNameAsync(userName);

            if (user is null)
            {
                user = new User
                {
                    Id = DeterministicGuid.Create($"ShopDemoUser_{shopIndex}"),
                    UserName = userName,
                    Mobile = $"0912000{shopIndex:0000}",
                    FirstName = "فروشنده",
                    LastName = $"نمونه {shopIndex}",
                    IsActive = true,
                    CreatedAt = seed.CreatedAt
                };

                var createResult = await userManager.CreateAsync(user, DemoShopPassword);
                if (!createResult.Succeeded)
                    continue; // اگر ساخت کاربر شکست خورد، از این فروشگاه صرف‌نظر می‌شود

                await userManager.AddToRoleAsync(user, ShopRole);
            }

            var shop = new Shop
            {
                Id = DeterministicGuid.Create($"ShopDemo_{shopIndex}"),
                UserId = user.Id,
                Apikey = ApiKeyGenerator.Create(),
                FirstName = "فروشنده",
                LastName = $"نمونه {shopIndex}",
                ShopName = seed.Name,
                ShopCategoryId = seed.ShopCategoryId,
                TextAddress = $"آدرس نمونه - {seed.Name} - پلاک {shopIndex}",
                CityId = seed.CityId,
                Latitude = 35.70m + shopIndex * 0.01m,
                Longitude = 51.40m + shopIndex * 0.01m,
                ReturnPolicy = "امکان بازگشت کالا تا ۷ روز پس از خرید در صورت سالم بودن بسته‌بندی.",
                CurrentStep = ShopRegistrationStep.Completed,
                VerificationStatus = ShopVerificationStatus.Approved,
                SubmittedAt = seed.CreatedAt,
                ReviewedAt = seed.CreatedAt,
                EnableStories = seed.EnableStories,
                CreatedAt = seed.CreatedAt
            };

            foreach (var wh in seed.WorkingHours)
            {
                shop.WorkingHours.Add(new ShopWorkingHour
                {
                    Id = Guid.NewGuid(),
                    ShopId = shop.Id,
                    DayOfWeek = wh.DayOfWeek,
                    OpenTime = wh.OpenTime,
                    CloseTime = wh.CloseTime,
                    IsClosed = wh.IsClosed,
                    CreatedAt = seed.CreatedAt
                });
            }

            context.Shops.Add(shop);

            foreach (var product in BuildProductsForShop(shopIndex, seed))
            {
                product.ShopId = shop.Id;
                context.Products.Add(product);
            }
        }

        await context.SaveChangesAsync(ct);
    }

    // ───────────── Helpers ─────────────

    private static List<WorkingHourSeed> BuildWorkingHours(bool closedAllDay)
    {
        var days = Enum.GetValues<DayOfWeek>();
        return days.Select(d => closedAllDay
                ? new WorkingHourSeed(d, null, null, true)
                : new WorkingHourSeed(d, TimeSpan.Zero, new TimeSpan(23, 59, 0), false))
            .ToList();
    }

    private static List<Product> BuildProductsForShop(int shopIndex, ShopSeed shopSeed)
    {
        var now = DateTime.UtcNow;

        // چهار محصول با ترکیب متفاوت از فیلترها برای هر فروشگاه
        var seeds = new List<ProductSeed>
        {
            new(
                Name: $"محصول ویژهٔ {shopSeed.Name} - شماره ۱",
                Brand: $"برند{shopIndex}A",
                BasePrice: 350_000 + shopIndex * 10_000,
                IsDiscountEnabled: true,
                DiscountPrice: 250_000 + shopIndex * 5_000, // تخفیف قابل توجه (MostDiscounted)
                StockQuantity: 25,
                IsSpecial: true,
                SoldCount: 120 + shopIndex, // پرفروش (BestSelling)
                ProductType: ProductType.Simple,
                CreatedAt: now.AddDays(-1)),

            new(
                Name: $"محصول پرفروش {shopSeed.Name} - شماره ۲",
                Brand: $"برند{shopIndex}B",
                BasePrice: 180_000 + shopIndex * 5_000,
                IsDiscountEnabled: false,
                DiscountPrice: null,
                StockQuantity: 40,
                IsSpecial: false,
                SoldCount: 80 + shopIndex,
                ProductType: ProductType.Simple,
                CreatedAt: now.AddDays(-15)),

            new(
                Name: $"محصول ناموجود {shopSeed.Name} - شماره ۳",
                Brand: $"برند{shopIndex}C",
                BasePrice: 220_000 + shopIndex * 5_000,
                IsDiscountEnabled: true,
                DiscountPrice: 200_000 + shopIndex * 5_000, // تخفیف کم
                StockQuantity: 0, // ناموجود
                IsSpecial: false,
                SoldCount: 5,
                ProductType: ProductType.Simple,
                CreatedAt: now.AddDays(-40)),

            new(
                Name: $"محصول تازه {shopSeed.Name} - شماره ۴",
                Brand: $"برند{shopIndex}D",
                BasePrice: 99_000 + shopIndex * 2_000,
                IsDiscountEnabled: false,
                DiscountPrice: null,
                StockQuantity: 60,
                IsSpecial: false,
                SoldCount: 0,
                ProductType: ProductType.Variable,
                CreatedAt: now.AddHours(-6)), // خیلی تازه
        };

        var productIndex = 0;
        return seeds.Select(s =>
        {
            productIndex++;
            return new Product
            {
                Id = DeterministicGuid.Create($"ProductDemo_{shopIndex}_{productIndex}"),
                Name = s.Name,
                Description = $"توضیحات نمونه برای {s.Name}. این محصول فقط برای تست API ساخته شده است.",
                Brand = s.Brand,
                BasePrice = s.BasePrice,
                IsDiscountEnabled = s.IsDiscountEnabled,
                DiscountPrice = s.DiscountPrice,
                StockQuantity = s.StockQuantity,
                HasSpecialPackaging = s.IsSpecial,
                IsSpecial = s.IsSpecial,
                SoldCount = s.SoldCount,
                Status = ProductStatus.Published,
                ProductType = s.ProductType,
                IsActiveByAdmin = true,
                CreatedAt = s.CreatedAt
            };
        }).ToList();
    }
}
