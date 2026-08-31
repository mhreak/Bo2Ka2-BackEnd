using FluentValidation;
using Bodokado.Application.App.ShopModule.Registration.DTOs;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.App.ShopModule.Registration.Validators;

public class ShopDetailsRequestValidator : AbstractValidator<ShopDetailsRequestDto>
{
    public ShopDetailsRequestValidator()
    {
        RuleFor(x => x.TextAddress).NotEmpty().WithMessage(MessageKeys.TextAddressRequired)
            .MaximumLength(1000).WithMessage(MessageKeys.TextAddressMaxLength);

        RuleFor(x => x.Latitude).InclusiveBetween(-90m, 90m).WithMessage(MessageKeys.LatitudeInvalid);
        RuleFor(x => x.Longitude).InclusiveBetween(-180m, 180m).WithMessage(MessageKeys.LongitudeInvalid);
    }
}