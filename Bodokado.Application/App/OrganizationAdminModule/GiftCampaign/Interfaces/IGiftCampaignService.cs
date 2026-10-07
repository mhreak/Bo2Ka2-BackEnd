using Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.DTOs;
using Bodokado.Application.Common.Pagination;

namespace Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.Interfaces;

public interface IGiftCampaignService
{
    Task<PagedResult<GiftCampaignListItemDto>> GetAllAsync(
        Guid organizationId, PaginationQuery query, CancellationToken ct = default);

    Task<GiftCampaignDetailDto> GetByIdAsync(
        Guid organizationId, Guid campaignId, CancellationToken ct = default);

    Task<GiftCampaignDetailDto> CreateAsync(
        Guid organizationId, CreateGiftCampaignRequestDto request, CancellationToken ct = default);

    Task<GiftCampaignDetailDto> UpdateAsync(
        Guid organizationId, Guid campaignId, UpdateGiftCampaignRequestDto request, CancellationToken ct = default);

    Task DeleteAsync(Guid organizationId, Guid campaignId, CancellationToken ct = default);

    Task<List<GiftCodeDto>> GenerateCodesAsync(
    Guid organizationId,
    Guid campaignId,
    GenerateGiftCodesRequestDto request,
    CancellationToken ct = default);

    Task<List<GiftCodeDto>> GetCodesAsync(
        Guid organizationId,
        Guid campaignId,
        bool? onlyUnused = null,
        CancellationToken ct = default);
}