using FluentValidation;
using Bodokado.Application.Common.File.DTOs;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.File.Validators;

public class FileAttachmentValidator : AbstractValidator<GenericUploadFileRequest>
{
    public FileAttachmentValidator()
    {
        RuleFor(x => x.File).NotNull();
        RuleFor(x => x.File.Length).GreaterThan(0);
    }
}
