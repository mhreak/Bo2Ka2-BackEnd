namespace Bodokado.Application.Administrator.File.DTOs;

public class AdminUploadFileRequest
{
    public Microsoft.AspNetCore.Http.IFormFile File { get; set; } = null!;
    public Bodokado.Domain.Enums.UploadFileType FileType { get; set; }
}
