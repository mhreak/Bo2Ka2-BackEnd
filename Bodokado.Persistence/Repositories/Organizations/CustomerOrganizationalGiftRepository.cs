using Microsoft.EntityFrameworkCore;
using Bodokado.Application.App.CustomerModule.OrganizationalGifts.Interfaces;
using Bodokado.Domain.Entities.Organizations;
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

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
