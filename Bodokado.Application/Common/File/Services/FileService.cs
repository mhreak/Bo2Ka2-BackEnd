using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.DTOs;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.File.Validators;
using Bodokado.Domain.Entities;
using Bodokado.Domain.Enums;

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
        if (userRole == "Admin")
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
}
