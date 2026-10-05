using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.Common.Auth.Interfaces;

public interface IGeneralOtpAuthService
{
    Task<SendOtpForAuthResponseDto> SendOtpAsync(GeneralOtpSendRequestDto request, CancellationToken cancellationToken = default);
    Task<GeneralOtpAuthResultDto> SubmitOtpAsync(GeneralOtpSubmitRequestDto request, string role, CancellationToken cancellationToken = default);
}