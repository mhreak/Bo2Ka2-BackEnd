using Bodokado.Domain.Common;

namespace Bodokado.Domain.Entities;

public class FileAsset : BaseEntity
{
    public string FileName { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public long Size { get; set; }
    public string Path { get; set; } = default!;
    public string UploadFileType { get; set; } = default!;
    public Guid OwnerId { get; set; }
    public Guid UploaderId { get; set; }
    public Bodokado.Domain.Enums.UploadFileType FileType { get; set; }
}
