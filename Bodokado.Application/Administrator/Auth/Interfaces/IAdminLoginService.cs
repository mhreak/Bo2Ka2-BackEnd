using Bodokado.Application.Administrator.Auth.DTOs;

namespace Bodokado.Application.Administrator.Auth.Interfaces;

public interface IAdminLoginService
{
    Task<AdminAuthResultDto> LoginAsync(AdminLoginRequestDto request);
}
