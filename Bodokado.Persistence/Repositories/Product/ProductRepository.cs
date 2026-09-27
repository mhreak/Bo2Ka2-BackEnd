using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.CustomerModule.Products.DTOs;
using Bodokado.Application.App.ShopModule.Products.DTOs;
using Bodokado.Application.App.ShopModule.Products.Interfaces;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.Products;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<Product?> GetByIdForShopAsync(Guid productId, Guid shopId, CancellationToken ct = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.ShopId == shopId && !p.IsDeleted, ct);
    }

    public async Task<Product?> GetByIdWithDetailsForShopAsync(Guid productId, Guid shopId, CancellationToken ct = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.ShopId == shopId && !p.IsDeleted, ct);
    }

    public async Task<PagedResult<Product>> GetPagedForShopAsync(Guid shopId, ProductListQuery query, CancellationToken ct = default)
    {
        var normalized = query.Normalize();
        IQueryable<Product> source = _context.Products
            .AsNoTracking()
            .Where(p => p.ShopId == shopId && !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            source = source.Where(p => p.Name.Contains(term) || (p.Brand != null && p.Brand.Contains(term)));
        }

        source = query.Filter switch
        {
            ProductListFilter.Special => source.Where(p => p.IsSpecial),
            ProductListFilter.BestSeller => source.Where(p => p.SoldCount > 0),
            _ => source
        };

        source = query.Filter == ProductListFilter.BestSeller
            ? source.OrderByDescending(p => p.SoldCount).ThenByDescending(p => p.CreatedAt)
            : source.OrderByDescending(p => p.CreatedAt);

        var totalCount = await source.CountAsync(ct);
        var items = await source
            .Skip(normalized.Skip)
            .Take(normalized.Take)
            .ToListAsync(ct);

        return PagedResult<Product>.Create(items, normalized, totalCount);
    }

    public async Task<Dictionary<Guid, int>> GetPublishedCountsByShopIdsAsync(IEnumerable<Guid> shopIds, CancellationToken ct = default)
    {
        var ids = shopIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, int>();

        return await _context.Products
            .AsNoTracking()
            .Where(p => ids.Contains(p.ShopId) && !p.IsDeleted &&
                        p.Status == ProductStatus.Published && p.IsActiveByAdmin)
            .GroupBy(p => p.ShopId)
            .Select(g => new { ShopId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ShopId, x => x.Count, ct);
    }

    public async Task<PagedResult<Product>> GetPagedForCustomerAsync(CustomerProductListQuery query, CancellationToken ct = default)
    {
        var normalized = query.Normalize();

        IQueryable<Product> source = _context.Products
            .AsNoTracking()
            .Include(p => p.Shop)
            .Include(p => p.MainImageFile)
            .Include(p => p.Images.Where(i => !i.IsDeleted))
            .Where(p => !p.IsDeleted &&
                        p.Status == ProductStatus.Published &&
                        p.IsActiveByAdmin &&
                        !p.Shop.IsDeleted &&
                        p.Shop.VerificationStatus == ShopVerificationStatus.Approved);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            source = source.Where(p => p.Name.Contains(term) || (p.Brand != null && p.Brand.Contains(term)));
        }

        if (query.ShopId.HasValue)
            source = source.Where(p => p.ShopId == query.ShopId.Value);

        if (query.ShopCategoryId.HasValue)
            source = source.Where(p => p.Shop.ShopCategoryId == query.ShopCategoryId.Value);

        if (!string.IsNullOrWhiteSpace(query.Brand))
            source = source.Where(p => p.Brand != null && p.Brand.Contains(query.Brand.Trim()));

        if (query.IsSpecial.HasValue)
            source = source.Where(p => p.IsSpecial == query.IsSpecial.Value);

        if (query.HasDiscount.HasValue)
            source = source.Where(p => p.IsDiscountEnabled == query.HasDiscount.Value);

        if (query.InStockOnly == true)
            source = source.Where(p => p.StockQuantity > 0);

        if (query.ProductType.HasValue)
            source = source.Where(p => p.ProductType == query.ProductType.Value);

        if (query.MinPrice.HasValue)
        {
            var min = query.MinPrice.Value;
            source = source.Where(p => (p.IsDiscountEnabled && p.DiscountPrice != null ? p.DiscountPrice.Value : p.BasePrice) >= min);
        }

        if (query.MaxPrice.HasValue)
        {
            var max = query.MaxPrice.Value;
            source = source.Where(p => (p.IsDiscountEnabled && p.DiscountPrice != null ? p.DiscountPrice.Value : p.BasePrice) <= max);
        }

        source = query.SortBy switch
        {
            ProductSortBy.PriceAsc => source.OrderBy(p => p.IsDiscountEnabled && p.DiscountPrice != null ? p.DiscountPrice.Value : p.BasePrice),
            ProductSortBy.PriceDesc => source.OrderByDescending(p => p.IsDiscountEnabled && p.DiscountPrice != null ? p.DiscountPrice.Value : p.BasePrice),
            ProductSortBy.BestSelling => source.OrderByDescending(p => p.SoldCount).ThenByDescending(p => p.CreatedAt),
            ProductSortBy.MostDiscounted => source
                .OrderByDescending(p => p.IsDiscountEnabled && p.DiscountPrice != null && p.BasePrice > 0
                    ? (p.BasePrice - p.DiscountPrice.Value) / p.BasePrice
                    : 0)
                .ThenByDescending(p => p.CreatedAt),
            _ => source.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await source.CountAsync(ct);
        var items = await source
            .Skip(normalized.Skip)
            .Take(normalized.Take)
            .ToListAsync(ct);

        return PagedResult<Product>.Create(items, normalized, totalCount);
    }

    public async Task<Product?> GetByIdForCustomerAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Shop)
                .ThenInclude(s => s.AvatarFile)
            .Include(p => p.Shop)
                .ThenInclude(s => s.WorkingHours.Where(w => !w.IsDeleted))
            .Include(p => p.MainImageFile)
            .Include(p => p.Images.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.File)
            .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted &&
                p.Status == ProductStatus.Published &&
                p.IsActiveByAdmin &&
                !p.Shop.IsDeleted &&
                p.Shop.VerificationStatus == ShopVerificationStatus.Approved, ct);
    }
}
