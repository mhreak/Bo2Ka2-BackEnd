using FluentValidation;
using Bodokado.Application.Common.Auth.DTOs;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.Common.Auth.Validators;

public class LoginByPasswordRequestValidator : AbstractValidator<LoginByPasswordRequestDto>
{
    public LoginByPasswordRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(MessageKeys.UsernameRequired)
            .MaximumLength(50);
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(MessageKeys.PasswordRequired);
    }
}
