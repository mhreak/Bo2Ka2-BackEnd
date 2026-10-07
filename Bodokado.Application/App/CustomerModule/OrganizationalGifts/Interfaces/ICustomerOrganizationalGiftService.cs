using Bodokado.Application.App.CustomerModule.OrganizationalGifts.DTOs;

namespace Bodokado.Application.App.CustomerModule.OrganizationalGifts.Interfaces;

public interface ICustomerOrganizationalGiftService
{
    Task<VerifyGiftCodeResponseDto> VerifyCodeAsync(
        Guid customerUserId,
        VerifyGiftCodeRequestDto request,
        CancellationToken ct = default);
}
