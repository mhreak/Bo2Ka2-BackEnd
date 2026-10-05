using FluentValidation;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.Common.Auth.Validators;

public class GeneralOtpSubmitRequestValidator : AbstractValidator<GeneralOtpSubmitRequestDto>
{
    public GeneralOtpSubmitRequestValidator()
    {
        RuleFor(request => request.Mobile)
            .NotEmpty().WithMessage(MessageKeys.PhoneNumberRequired)
            .Matches(@"^09\d{9}$").WithMessage(MessageKeys.IranianMobileNationalPattern);
        RuleFor(request => request.Code)
            .NotEmpty().WithMessage(MessageKeys.OtpCodeRequired)
            .Matches(@"^\d{5}$").WithMessage(MessageKeys.OtpCodeDigits);
    }
}