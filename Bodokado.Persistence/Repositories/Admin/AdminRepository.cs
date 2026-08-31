using Bodokado.Application.Administrator.Auth.Interfaces;
using Bodokado.Domain.Entities.Users;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;
    public AdminRepository(AppDbContext context) => _context = context;
    public async Task AddAsync(Admin admin) => await _context.Admins.AddAsync(admin);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
