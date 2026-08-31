using FluentValidation;
using Bodokado.Application.App.ShopModule.Registration.DTOs;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.App.ShopModule.Registration.Validators;

public class ShopFinalConfirmationRequestValidator : AbstractValidator<ShopFinalConfirmationRequestDto>
{
    public ShopFinalConfirmationRequestValidator()
    {
        RuleFor(x => x.ShabaNumber).IranianShaba();

        RuleFor(x => x.ReturnPolicy).NotEmpty().WithMessage(MessageKeys.ReturnPolicyRequired)
            .MaximumLength(2000).WithMessage(MessageKeys.ReturnPolicyMaxLength);

        RuleFor(x => x.WorkingHours)
            .NotEmpty().WithMessage(MessageKeys.WorkingHoursRequired)
            .Must(w => w.Select(d => d.DayOfWeek).Distinct().Count() == w.Count)
                .WithMessage(MessageKeys.WorkingHoursDuplicateDay);

        RuleForEach(x => x.WorkingHours).ChildRules(day =>
        {
            day.RuleFor(d => d).Must(d => d.IsClosed || (d.OpenTime.HasValue && d.CloseTime.HasValue && d.OpenTime < d.CloseTime))
                .WithMessage(MessageKeys.WorkingHoursRangeInvalid);
        });
    }
}