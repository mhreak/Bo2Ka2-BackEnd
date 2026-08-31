namespace Bodokado.Application.Common.File.DTOs;

public class FileUploadResponse
{
    public Guid Id { get; set; }
    public string Path { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string Extension { get; set; } = null!;
    public long Size { get; set; }
}
