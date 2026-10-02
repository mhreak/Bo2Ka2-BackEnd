namespace Bodokado.Application.Common.File.Interfaces;

public interface IRemoteFileImportService
{
    /// <summary>دانلود از URL و ذخیره در سیستم فایل پروژه — Id مربوط به FileAsset</summary>
    Task<Guid> ImportFromUrlAsync(string url, Guid uploaderUserId, CancellationToken ct = default);
}