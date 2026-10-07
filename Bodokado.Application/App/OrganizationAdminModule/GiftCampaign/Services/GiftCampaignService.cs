using Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.DTOs;
using Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Pagination;
using Bodokado.Domain.Entities.Organizations;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.Services;

public class GiftCampaignService : IGiftCampaignService
{
    private readonly IGiftCampaignRepository _repository;

    public GiftCampaignService(IGiftCampaignRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GiftCampaignListItemDto>> GetAllAsync(
        Guid organizationId, PaginationQuery query, CancellationToken ct = default)
    {
        var paged = await _repository.GetPagedForOrganizationAsync(organizationId, query, ct);
        var items = paged.Items.Select(MapListItem).ToList();
        return PagedResult<GiftCampaignListItemDto>.Create(items, query, paged.TotalCount);
    }

    public async Task<GiftCampaignDetailDto> GetByIdAsync(
        Guid organizationId, Guid campaignId, CancellationToken ct = default)
    {
        var campaign = await _repository.GetByIdWithConstraintsForOrganizationAsync(campaignId, organizationId, ct)
            ?? throw new NotFoundException(MessageKeys.GiftCampaignNotFound, "gift_campaign_not_found");

        return MapDetail(campaign);
    }

    public async Task<GiftCampaignDetailDto> CreateAsync(
        Guid organizationId, CreateGiftCampaignRequestDto request, CancellationToken ct = default)
    {
        await ValidateConstraintsAsync(organizationId, request.Constraints, ct);

        var campaign = new OrganizationalGiftCampaign
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            CampaignName = request.CampaignName.Trim(),
            StartDateTime = request.StartDateTime,
            FinishDateTime = request.FinishDateTime,
            OccasionType = request.OccasionType,
            OrganizationalMessageEnabled = request.OrganizationalMessageEnabled,
            MessageType = request.MessageType,
            OrganizationalMessage = request.OrganizationalMessage,
            OrganizationalMessageFileId = request.OrganizationalMessageFileId,
            IsActiveByAdmin = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var c in request.Constraints)
        {
            campaign.Constraints.Add(new OrganizationalGiftCampaignConstraint
            {
                Id = Guid.NewGuid(),
                OrganizationalGiftCampaignId = campaign.Id,
                ConstraintType = c.ConstraintType,
                OrganizationPersonnelCategoryId = c.OrganizationPersonnelCategoryId,
                Constraint = c.Constraint.Trim(),
                CreatedAt = DateTime.UtcNow
            });
        }

        await _repository.AddAsync(campaign);
        await _repository.SaveChangesAsync();

        return MapDetail(campaign);
    }

    public async Task<GiftCampaignDetailDto> UpdateAsync(
        Guid organizationId, Guid campaignId, UpdateGiftCampaignRequestDto request, CancellationToken ct = default)
    {
        var campaign = await _repository.GetByIdWithConstraintsForOrganizationAsync(campaignId, organizationId, ct)
            ?? throw new NotFoundException(MessageKeys.GiftCampaignNotFound, "gift_campaign_not_found");

        var hasIssuedCodes = await _repository.HasIssuedCodesAsync(campaignId, ct);
        if (hasIssuedCodes)
            throw new BadRequestException(MessageKeys.GiftCampaignAlreadyIssued, "gift_campaign_already_issued");

        await ValidateConstraintsAsync(organizationId, request.Constraints, ct);

        campaign.CampaignName = request.CampaignName.Trim();
        campaign.StartDateTime = request.StartDateTime;
        campaign.FinishDateTime = request.FinishDateTime;
        campaign.OccasionType = request.OccasionType;
        campaign.OrganizationalMessageEnabled = request.OrganizationalMessageEnabled;
        campaign.MessageType = request.MessageType;
        campaign.OrganizationalMessage = request.OrganizationalMessage;
        campaign.OrganizationalMessageFileId = request.OrganizationalMessageFileId;

        // جایگزینی کامل Constraints (ساده‌ترین روش برای Update)
        campaign.Constraints.Clear();
        foreach (var c in request.Constraints)
        {
            campaign.Constraints.Add(new OrganizationalGiftCampaignConstraint
            {
                Id = Guid.NewGuid(),
                OrganizationalGiftCampaignId = campaign.Id,
                ConstraintType = c.ConstraintType,
                OrganizationPersonnelCategoryId = c.OrganizationPersonnelCategoryId,
                Constraint = c.Constraint.Trim(),
                CreatedAt = DateTime.UtcNow
            });
        }

        _repository.Update(campaign);
        await _repository.SaveChangesAsync();

        return MapDetail(campaign);
    }

    public async Task DeleteAsync(Guid organizationId, Guid campaignId, CancellationToken ct = default)
    {
        var campaign = await _repository.GetByIdForOrganizationAsync(campaignId, organizationId, ct)
            ?? throw new NotFoundException(MessageKeys.GiftCampaignNotFound, "gift_campaign_not_found");

        campaign.IsActiveByAdmin = false;
        _repository.Update(campaign);
        await _repository.SaveChangesAsync();
    }

    // ───────────── Helpers ─────────────

    private async Task ValidateConstraintsAsync(
        Guid organizationId, List<CreateGiftCampaignConstraintDto> constraints, CancellationToken ct)
    {
        if (constraints.Count == 0) return;

        if (constraints.Any(c => c.FinishBeforeStart()))
            throw new BadRequestException(MessageKeys.ValidationFailed, "invalid_constraint");

        var categoryIds = constraints
            .Where(c => c.OrganizationPersonnelCategoryId.HasValue)
            .Select(c => c.OrganizationPersonnelCategoryId!.Value)
            .ToList();

        if (categoryIds.Count > 0)
        {
            var allBelong = await _repository.AllPersonnelCategoriesBelongToOrganizationAsync(categoryIds, organizationId, ct);
            if (!allBelong)
                throw new BadRequestException(MessageKeys.InvalidPersonnelCategory, "invalid_personnel_category");
        }

        foreach (var c in constraints)
        {
            if (c.ConstraintType == GiftCampaignConstraintType.PriceLimit)
            {
                if (!decimal.TryParse(c.Constraint, out var price) || price <= 0)
                    throw new BadRequestException(MessageKeys.InvalidPriceLimit, "invalid_price_limit");
            }
            else
            {
                var guidParts = c.Constraint.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (guidParts.Length == 0 || guidParts.Any(p => !Guid.TryParse(p, out _)))
                    throw new BadRequestException(MessageKeys.InvalidConstraintGuidList, "invalid_constraint_guid_list");
            }
        }
    }

    private static GiftCampaignListItemDto MapListItem(OrganizationalGiftCampaign c)
    {
        return new GiftCampaignListItemDto
        {
            Id = c.Id,
            CampaignName = c.CampaignName,
            StartDateTime = c.StartDateTime,
            FinishDateTime = c.FinishDateTime,
            OccasionType = c.OccasionType,
            IsActiveByAdmin = c.IsActiveByAdmin,
            ConstraintCount = c.Constraints.Count
        };
    }

    private static GiftCampaignDetailDto MapDetail(OrganizationalGiftCampaign c)
    {
        return new GiftCampaignDetailDto
        {
            Id = c.Id,
            OrganizationId = c.OrganizationId,
            CampaignName = c.CampaignName,
            StartDateTime = c.StartDateTime,
            FinishDateTime = c.FinishDateTime,
            OccasionType = c.OccasionType,
            OrganizationalMessageEnabled = c.OrganizationalMessageEnabled,
            MessageType = c.MessageType,
            OrganizationalMessage = c.OrganizationalMessage,
            OrganizationalMessageFileId = c.OrganizationalMessageFileId,
            IsActiveByAdmin = c.IsActiveByAdmin,
            Constraints = c.Constraints.Select(con => new GiftCampaignConstraintDto
            {
                Id = con.Id,
                ConstraintType = con.ConstraintType,
                OrganizationPersonnelCategoryId = con.OrganizationPersonnelCategoryId,
                OrganizationPersonnelCategoryName = con.OrganizationPersonnelCategory?.CategoryName,
                Constraint = con.Constraint
            }).ToList()
        };
    }

    public async Task<List<GiftCodeDto>> GenerateCodesAsync(
    Guid organizationId,
    Guid campaignId,
    GenerateGiftCodesRequestDto request,
    CancellationToken ct = default)
{
    if (request.Count < 1 || request.Count > 500)
        throw new BadRequestException(MessageKeys.ValidationFailed, "invalid_code_count");

    var campaign = await _repository.GetByIdForOrganizationAsync(organizationId, campaignId, ct)
        ?? throw new NotFoundException(MessageKeys.GiftCampaignNotFound, "campaign_not_found");

    if (!campaign.IsActiveByAdmin || campaign.IsDeleted)
        throw new BadRequestException(MessageKeys.ValidationFailed, "campaign_inactive");

    var list = new List<UserOrganizationalGiftCampaign>();
    var attempts = 0;

    while (list.Count < request.Count && attempts < request.Count * 5)
    {
        attempts++;
        var code = GenerateCode10();
        if (await _repository.GiftCodeExistsAsync(code, ct))
            continue;
        if (list.Any(x => x.GiftCode == code))
            continue;

        list.Add(new UserOrganizationalGiftCampaign
        {
            Id = Guid.NewGuid(),
            OrganizationalGiftCampaignId = campaignId,
            GiftCode = code,
            UserId = null,
            UsedAt = null,
            CreatedAt = DateTime.UtcNow
        });
    }

    if (list.Count < request.Count)
        throw new BadRequestException(MessageKeys.ValidationFailed, "code_generation_failed");

    await _repository.AddGiftCodesAsync(list, ct);
    

    return list.Select(MapCode).ToList();
}

    public async Task<List<GiftCodeDto>> GetCodesAsync(
        Guid organizationId,
        Guid campaignId,
        bool? onlyUnused = null,
        CancellationToken ct = default)
    {
        _ = await _repository.GetByIdForOrganizationAsync(organizationId, campaignId, ct)
            ?? throw new NotFoundException(MessageKeys.GiftCampaignNotFound, "campaign_not_found");

        var rows = await _repository.GetCodesAsync(campaignId, onlyUnused, ct);
        return rows.Select(MapCode).ToList();
    }

    private static GiftCodeDto MapCode(UserOrganizationalGiftCampaign x) => new()
    {
        Id = x.Id,
        Code = x.GiftCode,
        IsUsed = x.UserId.HasValue || x.UsedAt.HasValue,
        UsedByUserId = x.UserId,
        UsedAt = x.UsedAt
    };

    private static string GenerateCode10()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(10);
        return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
    }
}