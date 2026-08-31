using FluentValidation;
using Bodokado.Application.Common.File.DTOs;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.File.Validators;

public interface IFileAttachmentValidator : IValidator<GenericUploadFileRequest> { }

public class CommonFileValidators
{
    public static bool IsValidExtension(string fileName, IEnumerable<string> allowedExtensions)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return allowedExtensions.Contains(ext);
    }
}
