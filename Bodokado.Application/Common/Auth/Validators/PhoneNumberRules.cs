using FluentValidation;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.Common.Auth.Validators;

public static class PhoneNumberRules
{
    public const string Pattern = @"^\+989\d{9}$";

    public static IRuleBuilderOptions<T, string> IranianMobile<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage(MessageKeys.PhoneNumberRequired)
            .Matches(Pattern).WithMessage(MessageKeys.PhoneNumberPattern);
    }

    public static IRuleBuilderOptions<T, string> OtpCode<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage(MessageKeys.OtpCodeRequired)
            .Matches(@"^\d{5}$").WithMessage(MessageKeys.OtpCodeDigits);
    }
}
