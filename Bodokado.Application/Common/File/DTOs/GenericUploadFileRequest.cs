using Microsoft.AspNetCore.Http;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.Common.File.DTOs;

public class GenericUploadFileRequest
{
    public IFormFile File { get; set; } = null!;
    public UploadFileType FileType { get; set; }
}
