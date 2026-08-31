using FluentValidation;
using Bodokado.Application.App.ShopModule.Registration.DTOs;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.App.ShopModule.Registration.Validators;

public class ShopBasicInfoRequestValidator : AbstractValidator<ShopBasicInfoRequestDto>
{
    public ShopBasicInfoRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(MessageKeys.FirstNameRequired)
            .MaximumLength(50).WithMessage(MessageKeys.FirstNameMaxLength);

        RuleFor(x => x.LastName).NotEmpty().WithMessage(MessageKeys.LastNameRequired)
            .MaximumLength(50).WithMessage(MessageKeys.LastNameMaxLength);

        RuleFor(x => x.NationalCode).IranianNationalCode();

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage(MessageKeys.BirthDateRequired)
            .LessThan(DateTime.UtcNow).WithMessage(MessageKeys.BirthDateInFuture);

        RuleFor(x => x.ShopName).NotEmpty().WithMessage(MessageKeys.ShopNameRequired)
            .MaximumLength(200).WithMessage(MessageKeys.ShopNameMaxLength);

        RuleFor(x => x.ShopCategoryId).NotEmpty().WithMessage(MessageKeys.ShopCategoryRequired);
    }
}