using Bodokado.Application.App.Auth.DTOs;
using Bodokado.Application.Common.Auth.DTOs;

namespace Bodokado.Application.Common.Auth.Interfaces;

public interface ILoginService
{
    Task<AuthResultDto> LoginAsync(LoginRequestDto request);
}
