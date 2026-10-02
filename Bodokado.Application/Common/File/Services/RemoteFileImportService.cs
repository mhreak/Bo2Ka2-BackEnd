using System.Net.Http;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Enums;

namespace Bodokado.Infrastructure.Services.File;

public class RemoteFileImportService : IRemoteFileImportService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFileService _fileService;

    public RemoteFileImportService(IHttpClientFactory httpClientFactory, IFileService fileService)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
    }

    public async Task<Guid> ImportFromUrlAsync(string url, Guid uploaderUserId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(url) ||
            !Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new BadRequestException(MessageKeys.InvalidImageUrl, "invalid_image_url");
        }

        var client = _httpClientFactory.CreateClient("RemoteImage");
        using var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!response.IsSuccessStatusCode)
            throw new BadRequestException(MessageKeys.ImageDownloadFailed, "image_download_failed");

        var contentType = response.Content.Headers.ContentType?.MediaType;
        if (string.IsNullOrWhiteSpace(contentType) ||
            !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException(MessageKeys.InvalidImageContent, "invalid_image_content");

        const int maxBytes = 10 * 1024 * 1024;
        if (response.Content.Headers.ContentLength > maxBytes)
            throw new BadRequestException(MessageKeys.ImageTooLarge, "image_too_large");

        await using var source = await response.Content.ReadAsStreamAsync(ct);
        using var contentBuffer = new MemoryStream();
        var buffer = new byte[81920];
        while (true)
        {
            var bytesRead = await source.ReadAsync(buffer.AsMemory(), ct);
            if (bytesRead == 0)
                break;

            if (contentBuffer.Length + bytesRead > maxBytes)
                throw new BadRequestException(MessageKeys.ImageTooLarge, "image_too_large");

            await contentBuffer.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
        }

        var bytes = contentBuffer.ToArray();
        if (bytes.Length == 0)
            throw new BadRequestException(MessageKeys.ImageDownloadFailed, "image_download_failed");

        var fileName = Path.GetFileName(uri.LocalPath);
        if (string.IsNullOrWhiteSpace(fileName) || fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            fileName = $"wp-{Guid.NewGuid():N}{GuessExtension(contentType)}";

        // SaveFromBytesAsync stores the uploaded file and returns the saved FileAsset entity.
        // Return its identifier to satisfy the interface contract.
        var fileAsset = await _fileService.SaveFromBytesAsync(
            bytes,
            fileName,
            contentType,
            uploaderUserId,
            userRole: "Shop",
            fileType: UploadFileType.ProductImage,
            cancellationToken: ct);
        return fileAsset.Id;
    }

    private static string GuessExtension(string contentType) => contentType.ToLowerInvariant() switch
    {
        "image/png" => ".png",
        "image/webp" => ".webp",
        "image/gif" => ".gif",
        _ => ".jpg"
    };
}