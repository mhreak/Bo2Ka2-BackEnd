using Bodokado.Application.App.CustomerModule.Shops.DTOs;
using Bodokado.Application.App.CustomerModule.Shops.Interfaces;
using Bodokado.Application.App.ShopModule.Products.Interfaces;
using Bodokado.Application.App.ShopModule.Registration.DTOs;
using Bodokado.Application.App.ShopModule.Registration.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.CustomerModule.Shops.Services;

public class CustomerShopService : ICustomerShopService
{
    private readonly IShopRepository _shopRepository;
    private readonly IProductRepository _productRepository;

    public CustomerShopService(IShopRepository shopRepository, IProductRepository productRepository)
    {
        _shopRepository = shopRepository;
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ShopListItemDto>> GetAllAsync(ShopListQuery query, CancellationToken ct = default)
    {
        var paged = await _shopRepository.GetPagedForCustomerAsync(query, ct);
        var shopIds = paged.Items.Select(s => s.Id).ToList();
        var countMap = await _productRepository.GetPublishedCountsByShopIdsAsync(shopIds, ct);

        var items = paged.Items
            .Select(s => MapListItem(s, countMap.GetValueOrDefault(s.Id)))
            .ToList();
        return PagedResult<ShopListItemDto>.Create(items, query, paged.TotalCount);
    }

    public async Task<ShopDetailDto> GetByIdAsync(Guid shopId, CancellationToken ct = default)
    {
        var shop = await _shopRepository.GetApprovedByIdWithDetailsAsync(shopId, ct)
            ?? throw new NotFoundException(MessageKeys.ShopNotFound, "shop_not_found");

        var countMap = await _productRepository.GetPublishedCountsByShopIdsAsync(new[] { shopId }, ct);
        return MapDetail(shop, countMap.GetValueOrDefault(shopId));
    }

    // ───────────── Helpers ─────────────

    private static bool IsOpenNow(Shop shop)
    {
        var now = DateTime.UtcNow;
        var today = now.DayOfWeek;
        var timeNow = now.TimeOfDay;

        return shop.WorkingHours.Any(w =>
            !w.IsDeleted &&
            w.DayOfWeek == today &&
            !w.IsClosed &&
            w.OpenTime.HasValue && w.CloseTime.HasValue &&
            w.OpenTime.Value <= timeNow && timeNow <= w.CloseTime.Value);
    }

    private static ShopListItemDto MapListItem(Shop s, int productCount)
    {
        return new ShopListItemDto
        {
            Id = s.Id,
            ShopName = s.ShopName ?? string.Empty,
            AvatarFileId = s.AvatarFileId,
            AvatarPath = s.AvatarFile?.Path,
            CoverFileId = s.CoverFileId,
            CoverPath = s.CoverFile?.Path,
            ShopCategoryId = s.ShopCategoryId,
            ShopCategoryName = s.ShopCategory?.Name ?? string.Empty,
            CityId = s.CityId,
            CityName = s.City?.Name,
            TextAddress = s.TextAddress,
            IsOpenNow = IsOpenNow(s),
            EnableStories = s.EnableStories,
            ProductCount = productCount,
            CreatedAt = s.CreatedAt,
            IsNew = s.CreatedAt >= DateTime.UtcNow.AddDays(-30)
        };
    }

    private static ShopDetailDto MapDetail(Shop s, int productCount)
    {
        return new ShopDetailDto
        {
            Id = s.Id,
            ShopName = s.ShopName ?? string.Empty,
            AvatarFileId = s.AvatarFileId,
            AvatarPath = s.AvatarFile?.Path,
            CoverFileId = s.CoverFileId,
            CoverPath = s.CoverFile?.Path,
            ShopCategoryId = s.ShopCategoryId,
            ShopCategoryName = s.ShopCategory?.Name ?? string.Empty,
            TextAddress = s.TextAddress,
            CityId = s.CityId,
            CityName = s.City?.Name,
            Latitude = s.Latitude,
            Longitude = s.Longitude,
            ReturnPolicy = s.ReturnPolicy,
            IsOpenNow = IsOpenNow(s),
            EnableStories = s.EnableStories,
            ProductCount = productCount,
            WorkingHours = s.WorkingHours
                .Where(w => !w.IsDeleted)
                .OrderBy(w => w.DayOfWeek)
                .Select(w => new ShopWorkingHourDto
                {
                    DayOfWeek = w.DayOfWeek,
                    IsClosed = w.IsClosed,
                    OpenTime = w.OpenTime,
                    CloseTime = w.CloseTime
                }).ToList(),
            CreatedAt = s.CreatedAt
        };
    }
}
