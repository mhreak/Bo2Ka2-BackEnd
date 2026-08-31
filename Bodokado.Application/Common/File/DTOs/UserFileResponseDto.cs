namespace Bodokado.Application.App.File.DTOs;

public class UserFileResponseDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public long Size { get; set; }
    public string MimeType { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Url { get; set; } = default!;
}
