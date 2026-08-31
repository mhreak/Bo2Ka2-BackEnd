using Microsoft.AspNetCore.Identity;
using Bodokado.Application.Administrator.Auth.DTOs;
using Bodokado.Application.Administrator.Auth.Interfaces;
using Bodokado.Application.Common.Exceptions;
using Bodokado.Application.Common.Localization;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Application.Administrator.Auth.Services;

public class AdminRegisterService : IAdminRegisterService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IAdminRepository _adminRepository;

    public AdminRegisterService(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, IAdminRepository adminRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _adminRepository = adminRepository;
    }

    public async Task RegisterAsync(AdminRegisterRequestDto request)
    {
        var existing = await _userManager.FindByNameAsync(request.Username);
        if (existing != null)
            throw new BadRequestException(MessageKeys.UsernameAlreadyExists, "username_already_exists");
        if (!await _roleManager.RoleExistsAsync("Admin"))
            await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = "Admin" });
        var user = new User { UserName = request.Username, FirstName = request.FirstName, LastName = request.LastName, IsActive = true };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            throw new BadRequestException(MessageKeys.UserCreationError, "user_creation_failed", errors);
        }
        var admin = new Admin();
        await _adminRepository.AddAsync(admin);
        await _adminRepository.SaveChangesAsync();
        user.AdminId = admin.Id;
        await _userManager.AddToRoleAsync(user, "Admin");
        await _userManager.UpdateAsync(user);
    }
}
