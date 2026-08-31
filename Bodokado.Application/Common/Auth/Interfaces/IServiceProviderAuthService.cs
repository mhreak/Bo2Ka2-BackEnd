using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.App.ServiceProviderModule.Auth.Interfaces;

public interface IServiceProviderAuthService
{
    Task<SendOtpForAuthResponseDto> SendOtpForLoginAsync(SendOtpForAuthRequestDto request);
    Task<AuthResultDto> LoginOtpAsync(LoginOtpRequestDto request);
    Task<AuthResultDto> LoginByPasswordAsync(LoginByPasswordRequestDto request);
    Task<AuthResultDto> RegisterByMobileAsync(RegisterByMobileRequestDto request);
    Task<AuthResultDto> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken);
    Task<GoogleAuthResultDto> GoogleAuthAsync(GoogleAuthRequestDto request);
}
