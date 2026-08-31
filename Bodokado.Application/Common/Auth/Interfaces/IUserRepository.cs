using Microsoft.AspNetCore.Identity;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Application.Common.Auth.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<User?> GetByEmailAsync(string email);
}
