using Bodokado.Domain.Entities.Organizations;

namespace Bodokado.Application.App.CustomerModule.OrganizationalGifts.Interfaces;

public interface ICustomerOrganizationalGiftRepository
{
    Task<UserOrganizationalGiftCampaign?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
