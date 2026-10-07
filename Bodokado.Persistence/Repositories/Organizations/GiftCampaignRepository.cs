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
        public async Task AddGiftCodesAsync(IEnumerable<UserOrganizationalGiftCampaign> codes, CancellationToken ct = default)
        => await _context.Set<UserOrganizationalGiftCampaign>().AddRangeAsync(codes, ct);

    public Task<bool> GiftCodeExistsAsync(string code, CancellationToken ct = default)
        => _context.Set<UserOrganizationalGiftCampaign>().AnyAsync(x => x.GiftCode == code, ct);

    public async Task<List<UserOrganizationalGiftCampaign>> GetCodesAsync(
        Guid campaignId, bool? onlyUnused, CancellationToken ct = default)
    {
        var q = _context.Set<UserOrganizationalGiftCampaign>()
            .Where(x => x.OrganizationalGiftCampaignId == campaignId);

        if (onlyUnused == true)
            q = q.Where(x => x.UserId == null && x.UsedAt == null);
        else if (onlyUnused == false)
            q = q.Where(x => x.UserId != null || x.UsedAt != null);

        return await q.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
    }
}