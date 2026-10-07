using Bodokado.Application.Common.Interfaces.Repositories;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities.Organizations;

namespace Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.Interfaces;

public interface IGiftCampaignRepository : IGenericRepository<OrganizationalGiftCampaign>
{
    Task<PagedResult<OrganizationalGiftCampaign>> GetPagedForOrganizationAsync(
        Guid organizationId, PaginationQuery query, CancellationToken ct = default);

    Task<OrganizationalGiftCampaign?> GetByIdForOrganizationAsync(
        Guid id, Guid organizationId, CancellationToken ct = default);

    Task<OrganizationalGiftCampaign?> GetByIdWithConstraintsForOrganizationAsync(
        Guid id, Guid organizationId, CancellationToken ct = default);

    /// <summary>چک می‌کند آیا شناسه‌های دسته‌ی پرسنلی داده‌شده همگی متعلق به همین سازمان هستند</summary>
    Task<bool> AllPersonnelCategoriesBelongToOrganizationAsync(
        IEnumerable<Guid> categoryIds, Guid organizationId, CancellationToken ct = default);

    /// <summary>آیا برای این کمپین تا الان کدی صادر شده (برای جلوگیری از ویرایش بعد از صدور کد)</summary>
    Task<bool> HasIssuedCodesAsync(Guid campaignId, CancellationToken ct = default);
}