using FluentValidation;
using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.Common.Auth.Validators;

public class GoogleAuthRequestValidator : AbstractValidator<GoogleAuthRequestDto>
{
    public GoogleAuthRequestValidator()
    {
        RuleFor(x => x.IdToken).NotEmpty();
    }
}
