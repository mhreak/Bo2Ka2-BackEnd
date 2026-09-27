// Persistence/Repositories/Banners/BannerRepository.cs
using Microsoft.EntityFrameworkCore;
using Bodokado.Application.Administrator.Banners.Interfaces;
using Bodokado.Domain.Entities.Banners;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.Banners;

public class BannerRepository : BaseRepository<Banner>, IBannerRepository
{
    public BannerRepository(AppDbContext context) : base(context) { }

    public Task<Banner?> GetByIdWithImageAsync(Guid id, CancellationToken ct = default)
        => _context.Banners
            .Include(b => b.Image)
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, ct);

    public Task<List<Banner>> GetActiveOrderedAsync(CancellationToken ct = default)
        => _context.Banners
            .AsNoTracking()
            .Include(b => b.Image)
            .Where(b => !b.IsDeleted && b.IsActive)
            .OrderBy(b => b.ShowOrder)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync(ct);

    public Task<List<Banner>> GetAllOrderedAsync(CancellationToken ct = default)
        => _context.Banners
            .AsNoTracking()
            .Include(b => b.Image)
            .Where(b => !b.IsDeleted)
            .OrderBy(b => b.ShowOrder)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync(ct);
}