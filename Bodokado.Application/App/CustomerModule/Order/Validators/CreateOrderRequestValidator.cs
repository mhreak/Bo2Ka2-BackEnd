using FluentValidation;
using Bodokado.Application.App.CustomerModule.Orders.DTOs;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.App.CustomerModule.Orders.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequestDto>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage(MessageKeys.OrderItemsRequired);

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage(MessageKeys.OrderQuantityInvalid);
        });

        RuleFor(x => x.BuyerName)
            .NotEmpty().WithMessage(MessageKeys.BuyerNameRequired)
            .MaximumLength(150);

        RuleFor(x => x.BuyerPhone)
            .NotEmpty().WithMessage(MessageKeys.BuyerPhoneRequired)
            .MaximumLength(20);

        RuleFor(x => x.DeliveryAddress).MaximumLength(1000);
        RuleFor(x => x.GiftMessage).MaximumLength(1000);
    }
}
