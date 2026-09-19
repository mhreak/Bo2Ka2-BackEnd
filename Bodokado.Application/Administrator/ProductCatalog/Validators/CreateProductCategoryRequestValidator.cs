using Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;
using Bodokado.Application.Common.Localization;
using FluentValidation;

public class CreateProductCategoryRequestValidator : AbstractValidator<CreateProductCategoryRequestDto>
{
    public CreateProductCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageKeys.ProductCategoryNameRequired)
            .MaximumLength(150).WithMessage(MessageKeys.ProductCategoryNameMaxLength);
    }
}