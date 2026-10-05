using Microsoft.AspNetCore.Identity;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Application.Common.Auth.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<IReadOnlyList<User>> GetByMobileAsync(string nationalMobile, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email);
}
