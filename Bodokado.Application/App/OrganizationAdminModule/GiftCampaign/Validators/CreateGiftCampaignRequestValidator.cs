using FluentValidation;
using Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.DTOs;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.App.OrganizationAdminModule.GiftCampaigns.Validators;

public class CreateGiftCampaignRequestValidator : AbstractValidator<CreateGiftCampaignRequestDto>
{
    public CreateGiftCampaignRequestValidator()
    {
        RuleFor(x => x.CampaignName).NotEmpty().MaximumLength(200).WithMessage(MessageKeys.ValidationFailed);

        RuleFor(x => x.FinishDateTime)
            .GreaterThan(x => x.StartDateTime)
            .WithMessage(MessageKeys.ValidationFailed);

        RuleFor(x => x.OrganizationalMessage)
            .NotEmpty()
            .When(x => x.OrganizationalMessageEnabled && x.MessageType == Bodokado.Domain.Enums.OrganizationalMessageType.Text)
            .WithMessage(MessageKeys.ValidationFailed);
    }
}

public class UpdateGiftCampaignRequestValidator : AbstractValidator<UpdateGiftCampaignRequestDto>
{
    public UpdateGiftCampaignRequestValidator()
    {
        RuleFor(x => x.CampaignName).NotEmpty().MaximumLength(200).WithMessage(MessageKeys.ValidationFailed);

        RuleFor(x => x.FinishDateTime)
            .GreaterThan(x => x.StartDateTime)
            .WithMessage(MessageKeys.ValidationFailed);
    }
}