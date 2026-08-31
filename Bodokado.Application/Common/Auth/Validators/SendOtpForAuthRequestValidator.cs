using FluentValidation;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Auth.Validators;

namespace Bodokado.Application.Common.Auth.Validators;

public class SendOtpForAuthRequestValidator : AbstractValidator<SendOtpForAuthRequestDto>
{
    public SendOtpForAuthRequestValidator()
    {
        RuleFor(x => x.PhoneNumber).IranianMobile();
    }
}
