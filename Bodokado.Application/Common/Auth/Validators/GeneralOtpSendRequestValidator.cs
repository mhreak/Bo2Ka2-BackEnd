using FluentValidation;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.Common.Auth.Validators;

public class GeneralOtpSendRequestValidator : AbstractValidator<GeneralOtpSendRequestDto>
{
    public GeneralOtpSendRequestValidator()
    {
        RuleFor(request => request.Mobile)
            .NotEmpty().WithMessage(MessageKeys.PhoneNumberRequired)
            .Matches(@"^09\d{9}$").WithMessage(MessageKeys.IranianMobileNationalPattern);
    }
}