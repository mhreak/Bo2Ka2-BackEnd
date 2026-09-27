using Bodokado.Application.App.CustomerModule.Products.DTOs;
using Bodokado.Application.App.CustomerModule.Products.Interfaces;
using Bodokado.Application.App.ShopModule.Products.DTOs;
using Bodokado.Application.App.ShopModule.Products.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Entities.Shops;

namespace Bodokado.Application.App.CustomerModule.Products.Services;

public class CustomerProductService : ICustomerProductService
{
    private readonly IProductRepository _productRepository;

    public CustomerProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<CustomerProductListItemDto>> GetAllAsync(
        CustomerProductListQuery query, CancellationToken ct = default)
    {
        var paged = await _productRepository.GetPagedForCustomerAsync(query, ct);
        var items = paged.Items.Select(MapListItem).ToList();
        return PagedResult<CustomerProductListItemDto>.Create(items, query, paged.TotalCount);
    }

    public async Task<CustomerProductDetailDto> GetByIdAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await _productRepository.GetByIdForCustomerAsync(productId, ct)
            ?? throw new NotFoundException(MessageKeys.ProductNotFound, "product_not_found");

        return MapDetail(product);
    }

    // ───────────── Helpers ─────────────

    private static decimal GetEffectivePrice(Product p)
        => p.IsDiscountEnabled && p.DiscountPrice.HasValue ? p.DiscountPrice.Value : p.BasePrice;

    private static int? GetDiscountPercent(Product p)
    {
        if (!p.IsDiscountEnabled || !p.DiscountPrice.HasValue || p.BasePrice <= 0)
            return null;
        return (int)Math.Round((1 - (p.DiscountPrice.Value / p.BasePrice)) * 100);
    }

    private static bool IsShopOpenNow(Shop shop)
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

    private static string? GetPrimaryImagePath(Product p)
    {
        if (p.MainImageFile is not null)
            return p.MainImageFile.Path;

        var first = p.Images?.Where(i => !i.IsDeleted).OrderBy(i => i.SortOrder).FirstOrDefault();
        return first?.File?.Path;
    }

    private static CustomerProductListItemDto MapListItem(Product p)
    {
        return new CustomerProductListItemDto
        {
            Id = p.Id,
            Name = p.Name,
            Brand = p.Brand,
            BasePrice = p.BasePrice,
            IsDiscountEnabled = p.IsDiscountEnabled,
            DiscountPrice = p.DiscountPrice,
            EffectivePrice = GetEffectivePrice(p),
            DiscountPercent = GetDiscountPercent(p),
            IsInStock = p.StockQuantity > 0,
            IsSpecial = p.IsSpecial,
            SoldCount = p.SoldCount,
            PrimaryImagePath = GetPrimaryImagePath(p),
            ProductType = p.ProductType,
            ShopId = p.ShopId,
            ShopName = p.Shop?.ShopName ?? string.Empty,
            CreatedAt = p.CreatedAt
        };
    }

    private static CustomerProductDetailDto MapDetail(Product p)
    {
        var images = new List<ProductImageDto>();

        if (p.MainImageFileId.HasValue)
        {
            images.Add(new ProductImageDto
            {
                FileAssetId = p.MainImageFileId.Value,
                Path = p.MainImageFile?.Path,
                SortOrder = -1,
                IsPrimary = true
            });
        }

        foreach (var img in (p.Images ?? new List<ShopProductImage>())
                     .Where(i => !i.IsDeleted)
                     .OrderBy(i => i.SortOrder))
        {
            images.Add(new ProductImageDto
            {
                FileAssetId = img.FileId,
                Path = img.File?.Path,
                SortOrder = img.SortOrder,
                IsPrimary = false
            });
        }

        return new CustomerProductDetailDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Brand = p.Brand,
            WeightGrams = p.WeightGrams,
            LengthCm = p.LengthCm,
            WidthCm = p.WidthCm,
            HeightCm = p.HeightCm,
            BasePrice = p.BasePrice,
            IsDiscountEnabled = p.IsDiscountEnabled,
            DiscountPrice = p.DiscountPrice,
            EffectivePrice = GetEffectivePrice(p),
            DiscountPercent = GetDiscountPercent(p),
            IsInStock = p.StockQuantity > 0,
            HasSpecialPackaging = p.HasSpecialPackaging,
            IsSpecial = p.IsSpecial,
            SoldCount = p.SoldCount,
            ProductType = p.ProductType,
            MainImageFileId = p.MainImageFileId,
            Images = images,
            ShopId = p.ShopId,
            ShopName = p.Shop?.ShopName ?? string.Empty,
            ShopAvatarPath = p.Shop?.AvatarFile?.Path,
            ShopIsOpenNow = p.Shop is not null && IsShopOpenNow(p.Shop),
            CreatedAt = p.CreatedAt
        };
    }
}
