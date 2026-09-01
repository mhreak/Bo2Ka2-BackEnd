using Microsoft.EntityFrameworkCore;
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
            .Include(p => p.Colors.Where(c => !c.IsDeleted))
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
}
