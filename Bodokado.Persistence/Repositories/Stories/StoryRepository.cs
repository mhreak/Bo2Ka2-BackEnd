// Persistence/Repositories/Stories/StoryRepository.cs
using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.AdminModule.Stories.DTOs;
using Bodokado.Application.App.AdminModule.Stories.Interfaces;
using Bodokado.Domain.Entities.Stories;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.Stories;

public class StoryRepository : BaseRepository<Story>, IStoryRepository
{
    public StoryRepository(AppDbContext context) : base(context) { }

    public Task<Story?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return _context.Stories
            .Include(s => s.Shop)
            .Include(s => s.MediaFile)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);
    }

    public Task<List<Story>> GetVisibleAsync(
        StoryShowPlace? showPlace,
        Guid? shopId,
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var q = _context.Stories
            .AsNoTracking()
            .Include(s => s.Shop)
            .Include(s => s.MediaFile)
            .Where(s => !s.IsDeleted
                        && s.IsPublished
                        && !s.DisabledByAdmin
                        && s.IsActiveByAdmin
                        && (s.PublishDateTime == null || s.PublishDateTime <= now));

        if (showPlace.HasValue)
            q = q.Where(s => s.ShowPlace == showPlace.Value);

        if (shopId.HasValue)
            q = q.Where(s => s.ShopId == shopId.Value);

        return q
            .OrderBy(s => s.ShowOrder)
            .ThenByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }

    public Task<List<Story>> GetForAdminAsync(StoryListQuery query, CancellationToken ct = default)
    {
        var q = _context.Stories
            .AsNoTracking()
            .Include(s => s.Shop)
            .Include(s => s.MediaFile)
            .Where(s => !s.IsDeleted);

        if (query.ShowPlace.HasValue)
            q = q.Where(s => s.ShowPlace == query.ShowPlace.Value);

        if (query.ShopId.HasValue)
            q = q.Where(s => s.ShopId == query.ShopId.Value);

        if (query.IsPublished.HasValue)
            q = q.Where(s => s.IsPublished == query.IsPublished.Value);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);

        return q
            .OrderBy(s => s.ShowOrder)
            .ThenByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public Task<List<Story>> GetForShopAsync(Guid shopId, CancellationToken ct = default)
    {
        return _context.Stories
            .AsNoTracking()
            .Include(s => s.Shop)
            .Include(s => s.MediaFile)
            .Where(s => !s.IsDeleted && s.ShopId == shopId)
            .OrderBy(s => s.ShowOrder)
            .ThenByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }
}