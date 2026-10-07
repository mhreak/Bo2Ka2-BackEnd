// Persistence/Repositories/Banners/BannerRepository.cs
using Microsoft.EntityFrameworkCore;
using Bodokado.Application.Administrator.Banners.Interfaces;
using Bodokado.Domain.Entities.Banners;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;
using Bodokado.Domain.Enums;

namespace Bodokado.Persistence.Repositories.Banners;

public class BannerRepository : BaseRepository<Banner>, IBannerRepository
{
    public BannerRepository(AppDbContext context) : base(context) { }

    public Task<Banner?> GetByIdWithImageAsync(Guid id, CancellationToken ct = default)
        => _context.Banners
            .Include(b => b.Image)
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, ct);

    public async Task<List<Banner>> GetActiveOrderedAsync(
        BannerShowPlace? showPlace = null,
        CancellationToken ct = default)
    {
        var q = _context.Banners
            .AsNoTracking()
            .Include(b => b.Image)
            .Where(b => !b.IsDeleted && b.IsActive);

        if (showPlace.HasValue)
            q = q.Where(b => b.ShowPlace == showPlace.Value);

        return await q
            .OrderBy(b => b.ShowOrder)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<Banner>> GetAllOrderedAsync(
        BannerShowPlace? showPlace = null,
        CancellationToken ct = default)
    {
        var q = _context.Banners
            .AsNoTracking()
            .Include(b => b.Image)
            .Where(b => !b.IsDeleted);

        if (showPlace.HasValue)
            q = q.Where(b => b.ShowPlace == showPlace.Value);

        return await q
            .OrderBy(b => b.ShowOrder)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync(ct);
    }
}