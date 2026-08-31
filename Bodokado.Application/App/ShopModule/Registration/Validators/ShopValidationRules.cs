using FluentValidation;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.App.ShopModule.Registration.Validators;

public static class ShopValidationRules
{
    public static IRuleBuilderOptions<T, string> IranianNationalCode<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage(MessageKeys.NationalCodeRequired)
            .Must(IsValidNationalCode).WithMessage(MessageKeys.NationalCodeInvalid);
    }

    public static IRuleBuilderOptions<T, string> IranianShaba<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage(MessageKeys.ShabaRequired)
            .Matches(@"^IR\d{24}$").WithMessage(MessageKeys.ShabaInvalid);
    }

    private static bool IsValidNationalCode(string? nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode) || nationalCode.Length != 10 || !nationalCode.All(char.IsDigit))
            return false;

        if (nationalCode.Distinct().Count() == 1)
            return false;

        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += (int.Parse(nationalCode[i].ToString())) * (10 - i);

        var remainder = sum % 11;
        var checkDigit = int.Parse(nationalCode[9].ToString());

        return remainder < 2 ? checkDigit == remainder : checkDigit == 11 - remainder;
    }
}