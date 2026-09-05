using Bodokado.Application.App.ShopModule.Registration.DTOs;
using Bodokado.Application.App.ShopModule.Registration.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.File.Interfaces;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities.Shops;
using Bodokado.Domain.Enums;

namespace Bodokado.Application.App.ShopModule.Registration.Services;

public class ShopRegistrationService : IShopRegistrationService
{
    private readonly IShopRepository _shopRepository;
    private readonly IShopCategoryRepository _categoryRepository;
    private readonly IFileService _fileService;

    public ShopRegistrationService(IShopRepository shopRepository, IShopCategoryRepository categoryRepository, IFileService fileService)
    {
        _shopRepository = shopRepository;
        _categoryRepository = categoryRepository;
        _fileService = fileService;
    }

    public async Task<IReadOnlyList<ShopCategoryDto>> GetCategoriesAsync(string? search, CancellationToken ct = default)
    {
        var categories = await _categoryRepository.SearchAsync(search, ct);
        return categories.Select(MapCategory).ToList();
    }

    public async Task<ShopProfileDto> GetMyShopAsync(Guid userId, CancellationToken ct = default)
    {
        var shop = await _shopRepository.GetByUserIdWithDetailsAsync(userId, ct);
        if (shop is null)
            throw new BadRequestException(MessageKeys.ShopNotFound, "shop_not_found");

        return MapProfile(shop);
    }

    public async Task<ShopProfileDto> SubmitBasicInfoAsync(Guid userId, ShopBasicInfoRequestDto request, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdAsync(request.ShopCategoryId);
        if (category is null || !category.IsActive)
            throw new BadRequestException(MessageKeys.ShopCategoryNotFound, "shop_category_not_found");

        var shop = await _shopRepository.GetByUserIdAsync(userId, ct);
        if (shop is null)
        {
            shop = new Shop
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            await _shopRepository.AddAsync(shop);
        }
        else
        {
            EnsureEditable(shop);
        }

        if (await _shopRepository.NationalCodeExistsAsync(request.NationalCode, shop.Id, ct))
            throw new BadRequestException(MessageKeys.NationalCodeAlreadyExists, "national_code_already_exists");

        shop.FirstName = request.FirstName.Trim();
        shop.LastName = request.LastName.Trim();
        shop.NationalCode = request.NationalCode.Trim();
        shop.BirthDate = request.BirthDate;
        shop.ShopName = request.ShopName.Trim();
        shop.ShopCategoryId = request.ShopCategoryId;

        if (shop.CurrentStep < ShopRegistrationStep.ShopDetails)
            shop.CurrentStep = ShopRegistrationStep.ShopDetails;

        shop.UpdatedAt = DateTime.UtcNow;
        await _shopRepository.SaveChangesAsync();

        var saved = await _shopRepository.GetByUserIdWithDetailsAsync(userId, ct)
            ?? throw new BadRequestException(MessageKeys.ShopNotFound, "shop_not_found");
        return MapProfile(saved);
    }

    public async Task<ShopProfileDto> SubmitShopDetailsAsync(Guid userId, ShopDetailsRequestDto request, CancellationToken ct = default)
    {
        var shop = await _shopRepository.GetByUserIdAsync(userId, ct)
            ?? throw new BadRequestException(MessageKeys.ShopStepOrderInvalid, "shop_step_order_invalid");

        EnsureEditable(shop);

        if (shop.CurrentStep < ShopRegistrationStep.ShopDetails)
            throw new BadRequestException(MessageKeys.ShopStepOrderInvalid, "shop_step_order_invalid");

        if (request.AvatarFileId.HasValue)
            await EnsureFileOwnedByUser(request.AvatarFileId.Value, userId);
        if (request.CoverFileId.HasValue)
            await EnsureFileOwnedByUser(request.CoverFileId.Value, userId);

        shop.AvatarFileId = request.AvatarFileId;
        shop.CoverFileId = request.CoverFileId;
        shop.TextAddress = request.TextAddress.Trim();
        shop.CityId = request.CityId;
        shop.Latitude = request.Latitude;
        shop.Longitude = request.Longitude;

        if (shop.CurrentStep < ShopRegistrationStep.FinalConfirmation)
            shop.CurrentStep = ShopRegistrationStep.FinalConfirmation;

        shop.UpdatedAt = DateTime.UtcNow;
        await _shopRepository.SaveChangesAsync();

        var saved = await _shopRepository.GetByUserIdWithDetailsAsync(userId, ct)
            ?? throw new BadRequestException(MessageKeys.ShopNotFound, "shop_not_found");
        return MapProfile(saved);
    }

    public async Task<ShopProfileDto> SubmitFinalConfirmationAsync(Guid userId, ShopFinalConfirmationRequestDto request, CancellationToken ct = default)
    {
        var shop = await _shopRepository.GetByUserIdWithDetailsAsync(userId, ct)
            ?? throw new BadRequestException(MessageKeys.ShopStepOrderInvalid, "shop_step_order_invalid");

        EnsureEditable(shop);

        if (shop.CurrentStep < ShopRegistrationStep.FinalConfirmation)
            throw new BadRequestException(MessageKeys.ShopStepOrderInvalid, "shop_step_order_invalid");

        shop.ShabaNumber = request.ShabaNumber.Trim();
        shop.ReturnPolicy = request.ReturnPolicy.Trim();

        SyncWorkingHours(shop, request.WorkingHours);

        shop.CurrentStep = ShopRegistrationStep.Completed;
        shop.VerificationStatus = ShopVerificationStatus.Pending;
        shop.SubmittedAt = DateTime.UtcNow;
        shop.RejectionReason = null;
        shop.UpdatedAt = DateTime.UtcNow;

        await _shopRepository.SaveChangesAsync();

        var saved = await _shopRepository.GetByUserIdWithDetailsAsync(userId, ct)
            ?? throw new BadRequestException(MessageKeys.ShopNotFound, "shop_not_found");
        return MapProfile(saved);
    }

    private static void SyncWorkingHours(Shop shop, List<ShopWorkingHourDto> incoming)
    {
        shop.WorkingHours.RemoveAll(existing => incoming.All(i => i.DayOfWeek != existing.DayOfWeek));

        foreach (var day in incoming)
        {
            var existing = shop.WorkingHours.FirstOrDefault(w => w.DayOfWeek == day.DayOfWeek);
            if (existing is null)
            {
                shop.WorkingHours.Add(new ShopWorkingHour
                {
                    Id = Guid.NewGuid(),
                    ShopId = shop.Id,
                    DayOfWeek = day.DayOfWeek,
                    IsClosed = day.IsClosed,
                    OpenTime = day.IsClosed ? null : day.OpenTime,
                    CloseTime = day.IsClosed ? null : day.CloseTime,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.IsClosed = day.IsClosed;
                existing.OpenTime = day.IsClosed ? null : day.OpenTime;
                existing.CloseTime = day.IsClosed ? null : day.CloseTime;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    private async Task EnsureFileOwnedByUser(Guid fileId, Guid userId)
    {
        var file = await _fileService.GetByIdAsync(fileId);
        if (file is null)
            throw new BadRequestException(MessageKeys.FileNotFound, "file_not_found");
        if (file.UploaderId != userId)
            throw new BadRequestException(MessageKeys.FileNotOwnedByUser, "file_not_owned_by_user");
    }

    private static void EnsureEditable(Shop shop)
    {
        if (shop.VerificationStatus == ShopVerificationStatus.Pending)
            throw new BadRequestException(MessageKeys.ShopAlreadySubmitted, "shop_already_submitted");
        if (shop.VerificationStatus == ShopVerificationStatus.Approved)
            throw new BadRequestException(MessageKeys.ShopAlreadyApproved, "shop_already_approved");
    }

    private static ShopCategoryDto MapCategory(ShopCategory category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        IconKey = category.IconKey
    };

    private static ShopProfileDto MapProfile(Shop shop) => new()
    {
        Id = shop.Id,
        FirstName = shop.FirstName,
        LastName = shop.LastName,
        NationalCode = shop.NationalCode,
        BirthDate = shop.BirthDate,
        ShopName = shop.ShopName,
        ShopCategory = shop.ShopCategory is null ? null : MapCategory(shop.ShopCategory),
        AvatarFileId = shop.AvatarFileId,
        AvatarPath = shop.AvatarFile?.Path,
        CoverFileId = shop.CoverFileId,
        CoverPath = shop.CoverFile?.Path,
        TextAddress = shop.TextAddress,
        CityId = shop.CityId,
        Latitude = shop.Latitude,
        Longitude = shop.Longitude,
        ShabaNumber = shop.ShabaNumber,
        ReturnPolicy = shop.ReturnPolicy,
        WorkingHours = shop.WorkingHours
            .OrderBy(w => w.DayOfWeek)
            .Select(w => new ShopWorkingHourDto
            {
                DayOfWeek = w.DayOfWeek,
                IsClosed = w.IsClosed,
                OpenTime = w.OpenTime,
                CloseTime = w.CloseTime
            }).ToList(),
        CurrentStep = shop.CurrentStep,
        VerificationStatus = shop.VerificationStatus,
        RejectionReason = shop.RejectionReason,
        SubmittedAt = shop.SubmittedAt
    };
}