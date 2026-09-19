namespace Bodokado.Application.Administrator.Auth.Interfaces;

public interface IAdminRegisterService
{
    Task RegisterAsync(Bodokado.Application.Administrator.Auth.DTOs.AdminRegisterRequestDto request);
}
