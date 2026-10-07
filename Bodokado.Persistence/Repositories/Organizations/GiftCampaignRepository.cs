using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.Interfaces;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities.Organizations;
using Bodokado.Persistence.Context;
using Bodokado.Persistence.Repositories;

namespace Bodokado.Persistence.Repositories.Organizations;

public class GiftCampaignRepository : BaseRepository<OrganizationalGiftCampaign>, IGiftCampaignRepository
{
    public GiftCampaignRepository(AppDbContext context) : base(context) { }

    public async Task<PagedResult<OrganizationalGiftCampaign>> GetPagedForOrganizationAsync(
        Guid organizationId, PaginationQuery query, CancellationToken ct = default)
    {
        var normalized = query.Normalize();

        var source = _context.OrganizationalGiftCampaigns
            .AsNoTracking()
            .Include(c => c.Constraints)
            .Where(c => c.OrganizationId == organizationId)
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await source.CountAsync(ct);
        var items = await source
            .Skip(normalized.Skip)
            .Take(normalized.Take)
            .ToListAsync(ct);

        return PagedResult<OrganizationalGiftCampaign>.Create(items, normalized, totalCount);
    }

    public async Task<OrganizationalGiftCampaign?> GetByIdForOrganizationAsync(
        Guid id, Guid organizationId, CancellationToken ct = default)
    {
        return await _context.OrganizationalGiftCampaigns
            .FirstOrDefaultAsync(c => c.Id == id && c.OrganizationId == organizationId, ct);
    }

    public async Task<OrganizationalGiftCampaign?> GetByIdWithConstraintsForOrganizationAsync(
        Guid id, Guid organizationId, CancellationToken ct = default)
    {
        return await _context.OrganizationalGiftCampaigns
            .Include(c => c.Constraints)
                .ThenInclude(con => con.OrganizationPersonnelCategory)
            .FirstOrDefaultAsync(c => c.Id == id && c.OrganizationId == organizationId, ct);
    }

    public async Task<bool> AllPersonnelCategoriesBelongToOrganizationAsync(
        IEnumerable<Guid> categoryIds, Guid organizationId, CancellationToken ct = default)
    {
        var ids = categoryIds.Distinct().ToList();
        if (ids.Count == 0) return true;

        var matchCount = await _context.OrganizationPersonnelCategories
            .CountAsync(c => ids.Contains(c.Id) && c.OrganizationId == organizationId, ct);

        return matchCount == ids.Count;
    }

    public async Task<bool> HasIssuedCodesAsync(Guid campaignId, CancellationToken ct = default)
    {
        return await _context.Set<UserOrganizationalGiftCampaign>()
            .AnyAsync(x => x.OrganizationalGiftCampaignId == campaignId, ct);
    }
}