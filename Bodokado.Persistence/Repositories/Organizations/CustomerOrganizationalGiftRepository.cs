using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.CustomerModule.OrganizationalGifts.Interfaces;
using Bodokado.Domain.Entities.Organizations;
using Bodokado.Domain.Entities.Products;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Repositories.Organizations;

public class CustomerOrganizationalGiftRepository : BaseRepository<UserOrganizationalGiftCampaign>, ICustomerOrganizationalGiftRepository
{
    public CustomerOrganizationalGiftRepository(AppDbContext context) : base(context) { }

    public async Task<UserOrganizationalGiftCampaign?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var trimmed = code.Trim();

        return await _context.UserOrganizationalGiftCampaigns
            .Include(u => u.OrganizationalGiftCampaign)
                .ThenInclude(c => c.Constraints)
            .Include(u => u.OrganizationalGiftCampaign)
                .ThenInclude(c => c.Organization)
                    .ThenInclude(o => o.LogoFile)
            .Include(u => u.OrganizationalGiftCampaign)
                .ThenInclude(c => c.OrganizationalMessageFile)
            .FirstOrDefaultAsync(u => u.GiftCode == trimmed, ct);
    }

    public async Task<List<Shop>> GetShopsByIdsAsync(IEnumerable<Guid> shopIds, CancellationToken ct = default)
    {
        var ids = shopIds.Distinct().ToList();
        if (ids.Count == 0) return new List<Shop>();

        return await _context.Shops
            .AsNoTracking()
            .Include(s => s.AvatarFile)
            .Where(s => ids.Contains(s.Id) && !s.IsDeleted && s.VerificationStatus == ShopVerificationStatus.Approved)
            .ToListAsync(ct);
    }

    public async Task<List<Product>> GetProductsByIdsAsync(IEnumerable<Guid> productIds, CancellationToken ct = default)
    {
        var ids = productIds.Distinct().ToList();
        if (ids.Count == 0) return new List<Product>();

        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Shop)
            .Include(p => p.MainImageFile)
            .Include(p => p.Images.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.File)
            .Where(p => ids.Contains(p.Id) &&
                        !p.IsDeleted &&
                        p.Status == ProductStatus.Published &&
                        p.IsActiveByAdmin &&
                        !p.Shop.IsDeleted &&
                        p.Shop.VerificationStatus == ShopVerificationStatus.Approved)
            .ToListAsync(ct);
    }

    public async Task<List<ProductCategory>> GetCategoriesByIdsAsync(IEnumerable<Guid> categoryIds, CancellationToken ct = default)
    {
        var ids = categoryIds.Distinct().ToList();
        if (ids.Count == 0) return new List<ProductCategory>();

        return await _context.ProductCategories
            .AsNoTracking()
            .Include(c => c.Image)
            .Where(c => ids.Contains(c.Id) && !c.IsDeleted && c.IsActive)
            .ToListAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
