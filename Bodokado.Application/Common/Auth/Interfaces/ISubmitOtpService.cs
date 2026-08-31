using Bodokado.Application.App.Auth.DTOs;
using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.Common.Auth.Interfaces;

public interface ISubmitOtpService
{
    Task<AuthResultDto> SubmitOtpAsync(SubmitOtpRequestDto request);
}
