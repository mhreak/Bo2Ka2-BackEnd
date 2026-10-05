using Microsoft.EntityFrameworkCore;
using Bodokado.Application.Common.Auth.Interfaces;
using Bodokado.Domain.Entities.Users;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);
    }

    public async Task<IReadOnlyList<User>> GetByMobileAsync(string nationalMobile, CancellationToken cancellationToken = default)
    {
        var internationalMobile = $"+98{nationalMobile[1..]}";
        return await _context.Users
            .Where(user => user.Mobile == nationalMobile
                || user.PhoneNumber == nationalMobile
                || user.Mobile == internationalMobile
                || user.PhoneNumber == internationalMobile)
            .OrderBy(user => user.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }
}
