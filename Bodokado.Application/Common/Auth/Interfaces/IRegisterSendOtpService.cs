using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.Common.Auth.Interfaces;

public interface IRegisterSendOtpService
{
    Task<SendOtpForAuthResponseDto> ExecuteAsync(RegisterSendOtpRequestDto request);
}
