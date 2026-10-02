using System.Net.Http;
using System.Net.Http.Headers;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Enums;

namespace Bodokado.Infrastructure.Services.File;

public class RemoteFileImportService : IRemoteFileImportService
{
    private readonly HttpClient _httpClient;
    private readonly IFileService _fileService;

    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    public RemoteFileImportService(HttpClient httpClient, IFileService fileService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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

        _httpClient.Timeout = TimeSpan.FromSeconds(30);

        using var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!response.IsSuccessStatusCode)
            throw new BadRequestException(MessageKeys.ImageDownloadFailed, "image_download_failed");

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        if (!AllowedImageTypes.Contains(contentType))
            throw new BadRequestException(MessageKeys.InvalidImageContent, "invalid_image_content");

        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        if (bytes.Length == 0)
            throw new BadRequestException(MessageKeys.ImageDownloadFailed, "image_download_failed");

        const int maxBytes = 10 * 1024 * 1024;
        if (bytes.Length > maxBytes)
            throw new BadRequestException(MessageKeys.ImageTooLarge, "image_too_large");

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
            "Shop",
            UploadFileType.ProductImage,
            ct);
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