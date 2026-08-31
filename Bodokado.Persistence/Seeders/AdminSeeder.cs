using Microsoft.AspNetCore.Identity;
using Bodokado.Domain.Entities.Users;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Seeders;

public static class AdminSeeder
{
    public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = "Admin" });

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = "User" });

        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = "admin",
                FirstName = "System",
                LastName = "Admin",
                IsActive = true,
                Email = "admin@starter.local"
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                var admin = new Bodokado.Domain.Entities.Users.Admin();
                context.Admins.Add(admin);
                adminUser.AdminId = admin.Id;
                await userManager.UpdateAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
