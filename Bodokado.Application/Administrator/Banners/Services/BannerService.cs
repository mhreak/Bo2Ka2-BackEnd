// Application/Administrator/Banners/Services/BannerService.cs
using Bodokado.Application.Administrator.Banners.DTOs;
using Bodokado.Application.Administrator.Banners.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities.Banners;

namespace Bodokado.Application.Administrator.Banners.Services;

public class BannerService : IBannerService
{
    private readonly IBannerRepository _bannerRepository;
    private readonly IFileAssetRepository _fileAssetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BannerService(
        IBannerRepository bannerRepository,
        IFileAssetRepository fileAssetRepository,
        IUnitOfWork unitOfWork)
    {
        _bannerRepository = bannerRepository;
        _fileAssetRepository = fileAssetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BannerDto>> GetActiveAsync(CancellationToken ct = default)
    {
        var list = await _bannerRepository.GetActiveOrderedAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<List<BannerDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _bannerRepository.GetAllOrderedAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<BannerDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _bannerRepository.GetByIdWithImageAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.BannerNotFound, "banner_not_found");
        return Map(entity);
    }

    public async Task<BannerDto> CreateAsync(CreateBannerRequestDto request, CancellationToken ct = default)
    {
        Validate(request);
        await EnsureImageExistsIfSetAsync(request.ImageId);

        var entity = new Banner
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = NullIfWhite(request.Description),
            ImageId = request.ImageId,
            Link = NullIfWhite(request.Link),
            ShowOrder = request.ShowOrder,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _bannerRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(entity.Id, ct);
    }

    public async Task<BannerDto> UpdateAsync(Guid id, UpdateBannerRequestDto request, CancellationToken ct = default)
    {
        var entity = await _bannerRepository.GetByIdWithImageAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.BannerNotFound, "banner_not_found");

        Validate(request);
        await EnsureImageExistsIfSetAsync(request.ImageId);

        entity.Title = request.Title.Trim();
        entity.Description = NullIfWhite(request.Description);
        entity.ImageId = request.ImageId;
        entity.Link = NullIfWhite(request.Link);
        entity.ShowOrder = request.ShowOrder;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        _bannerRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _bannerRepository.GetByIdWithImageAsync(id, ct)
            ?? throw new NotFoundException(MessageKeys.BannerNotFound, "banner_not_found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _bannerRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static void Validate(CreateBannerRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new BadRequestException(MessageKeys.BannerTitleRequired, "banner_title_required");

        if (request.Title.Trim().Length > 200)
            throw new BadRequestException(MessageKeys.BannerTitleMaxLength, "banner_title_max_length");

        if (request.Description is { Length: > 1000 })
            throw new BadRequestException(MessageKeys.BannerDescriptionMaxLength, "banner_description_max_length");

        if (request.Link is { Length: > 500 })
            throw new BadRequestException(MessageKeys.BannerLinkMaxLength, "banner_link_max_length");
    }

    private async Task EnsureImageExistsIfSetAsync(Guid? imageId)
    {
        if (!imageId.HasValue)
            return;

        var file = await _fileAssetRepository.GetByIdAsync(imageId.Value);
        if (file is null || file.IsDeleted)
            throw new BadRequestException(MessageKeys.FileNotFound, "file_not_found");
    }

    private static string? NullIfWhite(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();

    private static BannerDto Map(Banner b) => new()
    {
        Id = b.Id,
        Title = b.Title,
        Description = b.Description,
        ImageId = b.ImageId,
        ImagePath = b.Image?.Path,
        Link = b.Link,
        ShowOrder = b.ShowOrder,
        IsActive = b.IsActive,
        CreatedAt = b.CreatedAt,
        UpdatedAt = b.UpdatedAt
    };
}