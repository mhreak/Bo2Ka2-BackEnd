using Bodokado.Domain.Entities.Users;

namespace Bodokado.Application.Administrator.Auth.Interfaces;

public interface IAdminRepository
{
    Task AddAsync(Admin admin);
    Task SaveChangesAsync();
}
