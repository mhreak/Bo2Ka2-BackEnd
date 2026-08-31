namespace Bodokado.Application.App.File.DTOs;

public class GenericUserUploadFileRequest
{
    public Microsoft.AspNetCore.Http.IFormFile File { get; set; } = null!;
    public Bodokado.Domain.Enums.UploadFileType FileType { get; set; }
}
