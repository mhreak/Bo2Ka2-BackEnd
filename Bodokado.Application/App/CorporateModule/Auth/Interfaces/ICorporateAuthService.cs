using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.App.CorporateModule.Auth.Interfaces;

public interface ICorporateAuthService
{
    Task<SendOtpForAuthResponseDto> SendOtpForRegisterAsync(RegisterSendOtpRequestDto request);
    Task<SendOtpForAuthResponseDto> SendOtpForLoginAsync(SendOtpForAuthRequestDto request);
    Task<AuthResultDto> LoginOtpAsync(LoginOtpRequestDto request);
    Task<AuthResultDto> LoginByPasswordAsync(LoginByPasswordRequestDto request);
    Task<AuthResultDto> RegisterByMobileAsync(RegisterByMobileRequestDto request);
    Task<AuthResultDto> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken);
    Task<GoogleAuthResultDto> GoogleAuthAsync(GoogleAuthRequestDto request);
}