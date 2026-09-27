using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.CustomerModule.Shops.DTOs;
using Bodokado.Application.App.ShopModule.Registration.Interfaces;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Repositories.Shops;

public class ShopRepository : BaseRepository<Shop>, IShopRepository
{
    public ShopRepository(AppDbContext context) : base(context) { }

    public async Task<Shop?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Shops
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);
    }

    public async Task<Shop?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Shops
            .Include(s => s.ShopCategory)
            .Include(s => s.City)
            .Include(s => s.AvatarFile)
            .Include(s => s.CoverFile)
            .Include(s => s.WorkingHours.Where(w => !w.IsDeleted))
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);
    }

    public async Task<bool> NationalCodeExistsAsync(string nationalCode, Guid excludeShopId, CancellationToken ct = default)
    {
        return await _context.Shops
            .AnyAsync(s => s.NationalCode == nationalCode && s.Id != excludeShopId && !s.IsDeleted, ct);
    }

    public async Task<PagedResult<Shop>> GetPagedForCustomerAsync(ShopListQuery query, CancellationToken ct = default)
    {
        var normalized = query.Normalize();
        var now = DateTime.UtcNow;
        var today = now.DayOfWeek;
        var timeNow = now.TimeOfDay;

        IQueryable<Shop> source = _context.Shops
            .AsNoTracking()
            .Include(s => s.ShopCategory)
            .Include(s => s.City)
            .Include(s => s.AvatarFile)
            .Include(s => s.CoverFile)
            .Include(s => s.WorkingHours.Where(w => !w.IsDeleted))
            .Where(s => !s.IsDeleted && s.VerificationStatus == ShopVerificationStatus.Approved);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            source = source.Where(s =>
                (s.ShopName != null && s.ShopName.Contains(term)) ||
                (s.TextAddress != null && s.TextAddress.Contains(term)));
        }

        if (query.ShopCategoryId.HasValue)
            source = source.Where(s => s.ShopCategoryId == query.ShopCategoryId.Value);

        if (query.CityId.HasValue)
            source = source.Where(s => s.CityId == query.CityId.Value);

        if (query.ProvinceId.HasValue)
            source = source.Where(s => s.City != null && s.City.ProvinceId == query.ProvinceId.Value);

        if (query.HasStories.HasValue)
            source = source.Where(s => s.EnableStories == query.HasStories.Value);

        if (query.OnlyNew == true)
        {
            var threshold = now.AddDays(-30);
            source = source.Where(s => s.CreatedAt >= threshold);
        }

        if (query.HasSpecialProducts == true)
        {
            source = source.Where(s => _context.Products.Any(p =>
                p.ShopId == s.Id && !p.IsDeleted &&
                p.Status == ProductStatus.Published && p.IsActiveByAdmin &&
                (p.IsSpecial || p.IsDiscountEnabled)));
        }

        if (query.OnlyOpenNow == true)
        {
            source = source.Where(s => s.WorkingHours.Any(w =>
                !w.IsDeleted &&
                w.DayOfWeek == today &&
                !w.IsClosed &&
                w.OpenTime != null && w.CloseTime != null &&
                w.OpenTime <= timeNow && timeNow <= w.CloseTime));
        }

        source = query.SortBy switch
        {
            ShopSortBy.NameAsc => source.OrderBy(s => s.ShopName),
            ShopSortBy.NameDesc => source.OrderByDescending(s => s.ShopName),
            ShopSortBy.MostProducts => source.OrderByDescending(s =>
                _context.Products.Count(p => p.ShopId == s.Id && !p.IsDeleted &&
                    p.Status == ProductStatus.Published && p.IsActiveByAdmin)),
            _ => source.OrderByDescending(s => s.CreatedAt)
        };

        var totalCount = await source.CountAsync(ct);
        var items = await source
            .Skip(normalized.Skip)
            .Take(normalized.Take)
            .ToListAsync(ct);

        return PagedResult<Shop>.Create(items, normalized, totalCount);
    }

    public async Task<Shop?> GetApprovedByIdWithDetailsAsync(Guid shopId, CancellationToken ct = default)
    {
        return await _context.Shops
            .AsNoTracking()
            .Include(s => s.ShopCategory)
            .Include(s => s.City)
            .Include(s => s.AvatarFile)
            .Include(s => s.CoverFile)
            .Include(s => s.WorkingHours.Where(w => !w.IsDeleted))
            .FirstOrDefaultAsync(s => s.Id == shopId && !s.IsDeleted &&
                s.VerificationStatus == ShopVerificationStatus.Approved, ct);
    }
}