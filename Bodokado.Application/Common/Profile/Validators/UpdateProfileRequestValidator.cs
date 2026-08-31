using FluentValidation;
using Bodokado.Application.Common.Localization;
using Bodokado.Application.Common.Profile.DTOs;

namespace Bodokado.Application.Common.Profile.Validators;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequestDto>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.FirstName))
            .WithMessage(MessageKeys.FirstNameMaxLength);
        RuleFor(x => x.LastName).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.LastName))
            .WithMessage(MessageKeys.LastNameMaxLength);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage(MessageKeys.ValidationFailed);
    }
}
