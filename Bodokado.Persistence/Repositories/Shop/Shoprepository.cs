using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.ShopModule.Registration.Interfaces;
using Bodokado.Domain.Entities.Shops;
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
            .Include(s => s.Province)
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
}