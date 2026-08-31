using FluentValidation;
using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.Common.Auth.Validators;

public class RegisterByMobileRequestValidator : AbstractValidator<RegisterByMobileRequestDto>
{
    public RegisterByMobileRequestValidator()
    {
        RuleFor(x => x.PhoneNumber).IranianMobile();
        RuleFor(x => x.Code).OtpCode();
    }
}
