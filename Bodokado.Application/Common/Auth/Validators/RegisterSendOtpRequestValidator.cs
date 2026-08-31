using FluentValidation;
using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.Common.Auth.Validators;

public class RegisterSendOtpRequestValidator : AbstractValidator<RegisterSendOtpRequestDto>
{
    public RegisterSendOtpRequestValidator()
    {
        RuleFor(x => x.PhoneNumber).IranianMobile();
    }
}
