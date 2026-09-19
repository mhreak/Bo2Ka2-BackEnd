using Bodokado.Application.App.AdminModule.ProductCatalog.DTOs;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Enums;
using FluentValidation;

public class CreateProductPropertyRequestValidator : AbstractValidator<CreateProductPropertyRequestDto>
{
    public CreateProductPropertyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageKeys.ProductPropertyNameRequired)
            .MaximumLength(150).WithMessage(MessageKeys.ProductPropertyNameMaxLength);

        RuleFor(x => x.Type)
            .Must(t => t is ProductPropertyType.Selectable or ProductPropertyType.Text)
            .WithMessage(MessageKeys.ProductPropertyTypeInvalid);
    }
}