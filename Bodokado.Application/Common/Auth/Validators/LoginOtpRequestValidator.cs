using FluentValidation;
using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.Common.Auth.Validators;

public class LoginOtpRequestValidator : AbstractValidator<LoginOtpRequestDto>
{
    public LoginOtpRequestValidator()
    {
        RuleFor(x => x.PhoneNumber).IranianMobile();
        RuleFor(x => x.Code).OtpCode();
    }
}
