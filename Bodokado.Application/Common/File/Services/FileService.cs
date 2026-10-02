using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.DTOs;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.File.Validators;
using Bodokado.Domain.Entities;
using Bodokado.Domain.Constants;
using Bodokado.Domain.Enums;
using System.Net.Mime;
using Bodokado.Application.Common.Localization;

namespace Bodokado.Application.Common.File.Services;

public class FileService : IFileService
{
    private readonly IFileAssetRepository _fileRepository;
    private readonly IValidator<GenericUploadFileRequest> _validator;
    private readonly string _storageRootPath;

    public FileService(IFileAssetRepository fileRepository, IValidator<GenericUploadFileRequest> validator, IHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        _fileRepository = fileRepository;
        _validator = validator;
        var configuredRoot = configuration["Storage:RootPath"];
        _storageRootPath = string.IsNullOrWhiteSpace(configuredRoot)
            ? Path.GetFullPath(Path.Combine(hostEnvironment.ContentRootPath, "..", "..", "uploads"))
            : Path.GetFullPath(configuredRoot);
    }

    public async Task<FileAsset> UploadForUserAsync(IFormFile file, Guid userId, string userRole, UploadFileType fileType)
    {
        var request = new GenericUploadFileRequest { File = file, FileType = fileType };
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            var error = result.Errors.First();
            throw new BadRequestException(error.ErrorMessage, error.ErrorCode);
        }
        ValidateUserFileType(userRole, fileType);
        if (ShouldDeleteOldFiles(fileType))
        {
            var existingActiveFiles = await _fileRepository.GetByUploaderIdAndTypeAsync(userId, fileType);
            foreach (var existing in existingActiveFiles)
            {
                existing.IsDeleted = true;
                _fileRepository.Update(existing);
            }
        }
        var relativeFolder = Path.Combine(userRole, userId.ToString(), fileType.ToString());
        var targetFolder = Path.Combine(_storageRootPath, relativeFolder);
        Directory.CreateDirectory(targetFolder);
        var fileExtension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(targetFolder, uniqueFileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);
        var entity = new FileAsset
        {
            Id = Guid.NewGuid(),
            FileName = uniqueFileName,
            Extension = fileExtension,
            Size = file.Length,
            UploadFileType = file.ContentType,
            Path = Path.Combine("uploads", relativeFolder, uniqueFileName).Replace("\\", "/"),
            UploaderId = userId,
            OwnerId = userId,
            FileType = fileType,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _fileRepository.AddAsync(entity);
        await _fileRepository.SaveChangesAsync();
        return entity;
    }

    private static bool ShouldDeleteOldFiles(UploadFileType fileType)
        => fileType == UploadFileType.Avatar || fileType == UploadFileType.Cover;

    private static void ValidateUserFileType(string userRole, UploadFileType fileType)
    {
        if (userRole == RoleNames.Admin)
            return;

        if ((userRole == RoleNames.Customer || userRole == RoleNames.Shop)
            && (fileType == UploadFileType.Avatar || fileType == UploadFileType.Cover || fileType == UploadFileType.TicketAttachment))
        {
            return;
        }

        throw new BadRequestException("InvalidRole", "INVALID_ROLE");
    }

    public Task<FileAsset?> GetByIdAsync(Guid id) => _fileRepository.GetByIdAsync(id);
    public Task<List<FileAsset>> GetByUploaderIdAsync(Guid uploaderId) => _fileRepository.GetByUploaderIdAsync(uploaderId);

    public async Task<bool> DeleteAsync(Guid id, Guid currentUserId, bool isAdmin = false)
    {
        var fileAsset = await _fileRepository.GetByIdAsync(id);
        if (fileAsset == null) return false;
        if (!isAdmin && fileAsset.UploaderId != currentUserId && fileAsset.OwnerId != currentUserId)
            throw new UnauthorizedAccessException("Failed");
        fileAsset.IsDeleted = true;
        _fileRepository.Update(fileAsset);
        await _fileRepository.SaveChangesAsync();
        return true;
    }

   public async Task<FileAsset> SaveFromBytesAsync(
    byte[] bytes,
    string fileName,
    string contentType,
    Guid userId,
    string userRole,
    UploadFileType fileType,
    CancellationToken cancellationToken = default)
{
    if (bytes is null || bytes.Length == 0)
        throw new BadRequestException(MessageKeys.ImageDownloadFailed, "image_download_failed");

    const long maxBytes = 10 * 1024 * 1024; // ۱۰ مگ — با validator آپلود هماهنگ کن اگر حد دیگری داری
    if (bytes.Length > maxBytes)
        throw new BadRequestException(MessageKeys.ImageTooLarge, "image_too_large");

    if (string.IsNullOrWhiteSpace(contentType) ||
        !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        throw new BadRequestException(MessageKeys.InvalidImageContent, "invalid_image_content");

    ValidateUserFileType(userRole, fileType);

    // برای ProductImage معمولاً فایل‌های قبلی را پاک نمی‌کنیم
    if (ShouldDeleteOldFiles(fileType))
    {
        var existingActiveFiles = await _fileRepository.GetByUploaderIdAndTypeAsync(userId, fileType);
        foreach (var existing in existingActiveFiles)
        {
            existing.IsDeleted = true;
            _fileRepository.Update(existing);
        }
    }

    var relativeFolder = Path.Combine(userRole, userId.ToString(), fileType.ToString());
    var targetFolder = Path.Combine(_storageRootPath, relativeFolder);
    Directory.CreateDirectory(targetFolder);

    var fileExtension = Path.GetExtension(fileName);
    if (string.IsNullOrWhiteSpace(fileExtension))
        fileExtension = GuessExtension(contentType);

    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
    var physicalPath = Path.Combine(targetFolder, uniqueFileName);

    await System.IO.File.WriteAllBytesAsync(physicalPath, bytes, cancellationToken);

    var entity = new FileAsset
    {
        Id = Guid.NewGuid(),
        FileName = uniqueFileName,
        Extension = fileExtension,
        Size = bytes.Length,
        UploadFileType = contentType,
        Path = Path.Combine("uploads", relativeFolder, uniqueFileName).Replace("\\", "/"),
        UploaderId = userId,
        OwnerId = userId,
        FileType = fileType,
        CreatedAt = DateTime.UtcNow,
        IsDeleted = false
    };

    await _fileRepository.AddAsync(entity);
    await _fileRepository.SaveChangesAsync();
    return entity;
}

private static string GuessExtension(string contentType) => contentType.ToLowerInvariant() switch
{
    "image/png" => ".png",
    "image/webp" => ".webp",
    "image/gif" => ".gif",
    "image/jpeg" => ".jpg",
    "image/jpg" => ".jpg",
    _ => ".jpg"
};
}

