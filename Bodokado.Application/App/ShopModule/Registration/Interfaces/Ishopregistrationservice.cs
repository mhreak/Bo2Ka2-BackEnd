using Bodokado.Application.App.ShopModule.Registration.DTOs;

namespace Bodokado.Application.App.ShopModule.Registration.Interfaces;

public interface IShopRegistrationService
{
    Task<IReadOnlyList<ShopCategoryDto>> GetCategoriesAsync(string? search, CancellationToken ct = default);
    Task<ShopProfileDto> GetMyShopAsync(Guid userId, CancellationToken ct = default);
    Task<ShopProfileDto> SubmitBasicInfoAsync(Guid userId, ShopBasicInfoRequestDto request, CancellationToken ct = default);
    Task<ShopProfileDto> SubmitShopDetailsAsync(Guid userId, ShopDetailsRequestDto request, CancellationToken ct = default);
    Task<ShopProfileDto> SubmitFinalConfirmationAsync(Guid userId, ShopFinalConfirmationRequestDto request, CancellationToken ct = default);
}