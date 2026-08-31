using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.Common.Auth.Interfaces;

public interface IRefreshAccessTokenService
{
    Task<AuthResultDto> RefreshAsync(RefreshTokenRequestDto request);
}
