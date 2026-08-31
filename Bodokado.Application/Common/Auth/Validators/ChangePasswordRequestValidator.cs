using FluentValidation;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.Common.Auth.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty().WithMessage(MessageKeys.PasswordRequired);
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(MessageKeys.PasswordRequired)
            .MinimumLength(6).WithMessage(MessageKeys.PasswordMinLength);
    }
}
