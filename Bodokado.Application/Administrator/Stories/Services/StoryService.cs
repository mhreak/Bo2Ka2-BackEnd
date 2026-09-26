using Bodokado.Application.App.AdminModule.Stories.DTOs;
using Bodokado.Application.App.AdminModule.Stories.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities.Stories;
using Bodokado.Domain.Enums;

public class StoryService : IStoryService
{
    private readonly IStoryRepository _storyRepository;
    private readonly IFileAssetRepository _fileAssetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StoryService(
        IStoryRepository storyRepository,
        IFileAssetRepository fileAssetRepository,
        IUnitOfWork unitOfWork)
    {
        _storyRepository = storyRepository;
        _fileAssetRepository = fileAssetRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>برای homepage / اپ — فقط استوری‌های معتبر</summary>
    public async Task<List<StoryDto>> GetVisibleAsync(
        StoryShowPlace? showPlace = null, Guid? shopId = null, CancellationToken ct = default)
    {
        var list = await _storyRepository.GetVisibleAsync(showPlace, shopId, ct);
        return await MapListAsync(list);
    }

    public async Task<List<StoryDto>> GetAllForAdminAsync(StoryListQuery query, CancellationToken ct = default)
    {
        var list = await _storyRepository.GetForAdminAsync(query, ct);
        return await MapListAsync(list);
    }

    public async Task<StoryDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _storyRepository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.StoryNotFound, "story_not_found");
        return await MapAsync(entity);
    }

    public async Task<StoryDto> CreateAsync(CreateStoryRequestDto request, bool isAdmin, CancellationToken ct = default)
    {
        ValidateButtonText(request.StoryButtonText);
        if (request.MediaFileId.HasValue)
            await EnsureFileExistsAsync(request.MediaFileId.Value);

        var entity = new Story
        {
            Id = Guid.NewGuid(),
            ShopId = request.ShopId,
            IsPublished = request.IsPublished,
            PublishDateTime = request.IsPublished
                ? (request.PublishDateTime ?? DateTime.UtcNow)
                : request.PublishDateTime,
            DisabledByAdmin = false,
            IsActiveByAdmin = true,
            MediaFileId = request.MediaFileId,
            StoryButtonText = NullIfWhite(request.StoryButtonText),
            StoryButtonClickEntityId = request.StoryButtonClickEntityId,
            StoryButtonClickActionType = request.StoryButtonClickActionType,
            ShowOrder = request.ShowOrder,
            ShowPlace = request.ShowPlace,
            CreatedAt = DateTime.UtcNow
        };

        await _storyRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(entity.Id, ct);
    }

    public async Task<StoryDto> UpdateAsync(Guid id, UpdateStoryRequestDto request, bool isAdmin, CancellationToken ct = default)
    {
        var entity = await _storyRepository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.StoryNotFound, "story_not_found");

        ValidateButtonText(request.StoryButtonText);
        if (request.MediaFileId.HasValue)
            await EnsureFileExistsAsync(request.MediaFileId.Value);

        entity.ShopId = request.ShopId;
        entity.IsPublished = request.IsPublished;
        entity.PublishDateTime = request.IsPublished
            ? (request.PublishDateTime ?? entity.PublishDateTime ?? DateTime.UtcNow)
            : request.PublishDateTime;
        entity.MediaFileId = request.MediaFileId;
        entity.StoryButtonText = NullIfWhite(request.StoryButtonText);
        entity.StoryButtonClickEntityId = request.StoryButtonClickEntityId;
        entity.StoryButtonClickActionType = request.StoryButtonClickActionType;
        entity.ShowOrder = request.ShowOrder;
        entity.ShowPlace = request.ShowPlace;
        entity.UpdatedAt = DateTime.UtcNow;

        // فقط ادمین
        if (isAdmin)
        {
            if (request.DisabledByAdmin.HasValue)
                entity.DisabledByAdmin = request.DisabledByAdmin.Value;
            if (request.IsActiveByAdmin.HasValue)
                entity.IsActiveByAdmin = request.IsActiveByAdmin.Value;
        }

        _storyRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _storyRepository.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.StoryNotFound, "story_not_found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _storyRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static void ValidateButtonText(string? text)
    {
        if (text is not null && text.Trim().Length > 30)
            throw new BadRequestException(MessageKeys.StoryButtonTextMaxLength, "story_button_text_max_length");
    }

    private async Task EnsureFileExistsAsync(Guid fileId)
    {
        var file = await _fileAssetRepository.GetByIdAsync(fileId);
        if (file is null || file.IsDeleted)
            throw new BadRequestException(MessageKeys.FileNotFound, "file_not_found");
    }

    private static string? NullIfWhite(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();

    private async Task<List<StoryDto>> MapListAsync(List<Story> list)
    {
        var result = new List<StoryDto>(list.Count);
        foreach (var s in list)
            result.Add(await MapAsync(s));
        return result;
    }

    private async Task<StoryDto> MapAsync(Story s)
    {
        string? path = null;
        string? fileName = null;
        string? extension = null;
        string? uploadFileType = null;
        if (s.MediaFileId.HasValue)
        {
            var file = s.MediaFile ?? await _fileAssetRepository.GetByIdAsync(s.MediaFileId.Value);
            path = file?.Path;
            fileName = file?.FileName;
            extension = file?.Extension;
            uploadFileType = file?.UploadFileType;
        }

        return new StoryDto
        {
            Id = s.Id,
            ShopId = s.ShopId,
            ShopName = s.Shop?.ShopName,
            IsPublished = s.IsPublished,
            PublishDateTime = s.PublishDateTime,
            DisabledByAdmin = s.DisabledByAdmin,
            IsActiveByAdmin = s.IsActiveByAdmin,
            MediaFileId = s.MediaFileId,
            MediaPath = path,
            MediaFileName = fileName,
            MediaExtension = extension,
            MediaUploadFileType = uploadFileType,
            StoryButtonText = s.StoryButtonText,
            StoryButtonClickEntityId = s.StoryButtonClickEntityId,
            StoryButtonClickActionType = s.StoryButtonClickActionType,
            ShowOrder = s.ShowOrder,
            ShowPlace = s.ShowPlace,
            CreatedAt = s.CreatedAt
        };
    }
}