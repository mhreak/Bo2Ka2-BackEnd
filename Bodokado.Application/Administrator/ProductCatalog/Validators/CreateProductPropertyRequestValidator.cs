using Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Enums;
using FluentValidation;

public class CreateProductAttributeRequestValidator : AbstractValidator<CreateProductAttributeRequestDto>
{
    public CreateProductAttributeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageKeys.ProductPropertyNameRequired)
            .MaximumLength(150).WithMessage(MessageKeys.ProductPropertyNameMaxLength);

        RuleFor(x => x.Type)
            .Must(t => t is ProductAttributeType.Selectable or ProductAttributeType.Text)
            .WithMessage(MessageKeys.ProductPropertyTypeInvalid);
    }
}