using Bodokado.Application.App.CustomerModule.OrganizationalGifts.DTOs;
using Bodokado.Application.App.CustomerModule.OrganizationalGifts.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities.Organizations;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.CustomerModule.OrganizationalGifts.Services;

public class CustomerOrganizationalGiftService : ICustomerOrganizationalGiftService
{
    private readonly ICustomerOrganizationalGiftRepository _repository;

    public CustomerOrganizationalGiftService(ICustomerOrganizationalGiftRepository repository)
    {
        _repository = repository;
    }

    public async Task<VerifyGiftCodeResponseDto> VerifyCodeAsync(
        Guid customerUserId,
        VerifyGiftCodeRequestDto request,
        CancellationToken ct = default)
    {
        if (customerUserId == Guid.Empty)
            throw new BadRequestException(MessageKeys.UserNotFound, "user_not_found");

        if (string.IsNullOrWhiteSpace(request?.Code))
            throw new BadRequestException(MessageKeys.InvalidGiftCode, "invalid_gift_code");

        var normalizedCode = request.Code.Trim().ToUpperInvariant();
        if (normalizedCode.Length != 10)
            throw new BadRequestException(MessageKeys.InvalidGiftCode, "invalid_gift_code");

        // 1) Find UserOrganizationalGiftCampaign by GiftCode
        var giftCodeEntity = await _repository.GetByCodeAsync(normalizedCode, ct);
        if (giftCodeEntity == null || giftCodeEntity.IsDeleted)
            throw new NotFoundException(MessageKeys.GiftCodeNotFound, "gift_code_not_found");

        // 2) Code must be unused: UserId == null AND UsedAt == null
        if (giftCodeEntity.UserId.HasValue || giftCodeEntity.UsedAt.HasValue)
            throw new BadRequestException(MessageKeys.GiftCodeAlreadyUsed, "gift_code_already_used");

        // 3) Load related OrganizationalGiftCampaign with Constraints, Organization, LogoFile, OrganizationalMessageFile
        var campaign = giftCodeEntity.OrganizationalGiftCampaign;
        if (campaign == null)
            throw new NotFoundException(MessageKeys.GiftCampaignNotFound, "campaign_not_found");

        // 4) Campaign must exist, !IsDeleted, IsActiveByAdmin, now UTC between StartDateTime and FinishDateTime
        if (campaign.IsDeleted || !campaign.IsActiveByAdmin)
            throw new BadRequestException(MessageKeys.GiftCampaignInactive, "campaign_inactive");

        var nowUtc = DateTime.UtcNow;
        if (nowUtc < campaign.StartDateTime || nowUtc > campaign.FinishDateTime)
            throw new BadRequestException(MessageKeys.GiftCampaignNotInPeriod, "campaign_not_in_period");

        // 5) Resolve MaxBudget from constraints where ConstraintType == PriceLimit (parse Constraint as long > 0).
        // Prefer constraint with OrganizationPersonnelCategoryId == null if multiple; otherwise first valid
        var priceConstraints = campaign.Constraints
            .Where(c => c.ConstraintType == GiftCampaignConstraintType.PriceLimit && !c.IsDeleted)
            .ToList();

        long? resolvedMaxBudget = null;

        var defaultPriceConstraint = priceConstraints
            .FirstOrDefault(c => c.OrganizationPersonnelCategoryId == null && long.TryParse(c.Constraint, out var val) && val > 0);

        if (defaultPriceConstraint != null && long.TryParse(defaultPriceConstraint.Constraint, out var defaultVal) && defaultVal > 0)
        {
            resolvedMaxBudget = defaultVal;
        }
        else
        {
            var firstValidConstraint = priceConstraints
                .FirstOrDefault(c => long.TryParse(c.Constraint, out var val) && val > 0);

            if (firstValidConstraint != null && long.TryParse(firstValidConstraint.Constraint, out var firstVal) && firstVal > 0)
            {
                resolvedMaxBudget = firstVal;
            }
        }

        if (!resolvedMaxBudget.HasValue)
            throw new BadRequestException(MessageKeys.InvalidPriceLimit, "invalid_price_limit");

        var maxBudget = resolvedMaxBudget.Value;

        // 6) Parse AllowedShopIds / AllowedProductIds / AllowedCategoryIds from constraints ShopLimit / ProductLimit / CategoryLimit
        var allowedShopIds = ParseGuidList(campaign.Constraints, GiftCampaignConstraintType.ShopLimit);
        var allowedProductIds = ParseGuidList(campaign.Constraints, GiftCampaignConstraintType.ProductLimit);
        var allowedCategoryIds = ParseGuidList(campaign.Constraints, GiftCampaignConstraintType.CategoryLimit);

        // 7) Claim the code: set UserId = current customer user id, UsedAt = UtcNow, UpdatedAt = UtcNow, SaveChanges
        giftCodeEntity.UserId = customerUserId;
        giftCodeEntity.UsedAt = nowUtc;
        giftCodeEntity.UpdatedAt = nowUtc;

        await _repository.SaveChangesAsync(ct);

        // 8) Return VerifyGiftCodeResponseDto with campaign info, budget, message, allowed id lists
        return new VerifyGiftCodeResponseDto
        {
            CampaignId = campaign.Id,
            CampaignName = campaign.CampaignName,
            OrganizationId = campaign.OrganizationId,
            OrganizationName = campaign.Organization?.OrganizationName,
            OrganizationLogoPath = campaign.Organization?.LogoFile?.Path,
            OccasionType = campaign.OccasionType,
            GiftCode = giftCodeEntity.GiftCode,
            MaxBudget = maxBudget,
            RemainingBudget = maxBudget,
            Message = new OrgGiftMessageDto
            {
                Enabled = campaign.OrganizationalMessageEnabled,
                Type = campaign.MessageType,
                Text = campaign.OrganizationalMessage,
                FileId = campaign.OrganizationalMessageFileId,
                FilePath = campaign.OrganizationalMessageFile?.Path
            },
            AllowedShopIds = allowedShopIds,
            AllowedProductIds = allowedProductIds,
            AllowedCategoryIds = allowedCategoryIds
        };
    }

    private static List<Guid> ParseGuidList(
        IEnumerable<OrganizationalGiftCampaignConstraint> constraints,
        GiftCampaignConstraintType type)
    {
        var result = new List<Guid>();
        var matching = constraints.Where(c => c.ConstraintType == type && !c.IsDeleted);

        foreach (var c in matching)
        {
            if (string.IsNullOrWhiteSpace(c.Constraint))
                continue;

            var parts = c.Constraint.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var part in parts)
            {
                if (Guid.TryParse(part, out var id) && !result.Contains(id))
                {
                    result.Add(id);
                }
            }
        }

        return result;
    }
}
