using FluentValidation;
using Bodokado.Application.App.ShopModule.Products.DTOs;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.App.ShopModule.Products.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequestDto>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageKeys.ProductNameRequired)
            .MaximumLength(200).WithMessage(MessageKeys.ProductNameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage(MessageKeys.ProductDescriptionMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Brand)
            .MaximumLength(100).WithMessage(MessageKeys.ProductBrandMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Brand));

        RuleFor(x => x.BasePrice)
            .GreaterThan(0).WithMessage(MessageKeys.ProductBasePriceInvalid);

        RuleFor(x => x.DiscountPrice)
            .NotNull().WithMessage(MessageKeys.ProductDiscountPriceRequired)
            .GreaterThan(0).WithMessage(MessageKeys.ProductDiscountPriceInvalid)
            .LessThan(x => x.BasePrice).WithMessage(MessageKeys.ProductDiscountPriceInvalid)
            .When(x => x.IsDiscountEnabled);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage(MessageKeys.ProductStockInvalid);

        RuleFor(x => x.WeightGrams)
            .GreaterThan(0).WithMessage(MessageKeys.ProductWeightInvalid)
            .When(x => x.WeightGrams.HasValue);

        RuleFor(x => x.LengthCm)
            .GreaterThan(0).WithMessage(MessageKeys.ProductDimensionInvalid)
            .When(x => x.LengthCm.HasValue);

        RuleFor(x => x.WidthCm)
            .GreaterThan(0).WithMessage(MessageKeys.ProductDimensionInvalid)
            .When(x => x.WidthCm.HasValue);

        RuleFor(x => x.HeightCm)
            .GreaterThan(0).WithMessage(MessageKeys.ProductDimensionInvalid)
            .When(x => x.HeightCm.HasValue);

        RuleForEach(x => x.Colors)
            .ChildRules(c =>
            {
                c.RuleFor(y => y.Name)
                    .NotEmpty().WithMessage(MessageKeys.ProductColorNameRequired)
                    .MaximumLength(50).WithMessage(MessageKeys.ProductColorNameMaxLength);
            });

        RuleFor(x => x.ImageFileIds)
            .Must(ids => ids == null || ids.Count <= 10)
            .WithMessage(MessageKeys.ProductImagesMaxCount);
    }
}
