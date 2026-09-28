using Microsoft.AspNetCore.Identity;
using Bodokado.Domain.Constants;
using Bodokado.Domain.Entities.Users;

namespace Bodokado.Persistence.Seeders;

public static class RoleSeeder
{
    private static readonly string[] Roles = { RoleNames.Admin, RoleNames.Shop, RoleNames.Customer, RoleNames.UserOrganization };

    public static async Task SeedAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var role in Roles)
        {
            var existingRole = await roleManager.FindByNameAsync(role);
            if (existingRole is null)
            {
                var createResult = await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                EnsureSucceeded(createResult, $"Could not create role '{role}'");
            }
            else if (existingRole.Name != role)
            {
                existingRole.Name = role;
                var updateResult = await roleManager.UpdateAsync(existingRole);
                EnsureSucceeded(updateResult, $"Could not normalize role '{role}'");
            }
        }
    }

    public static async Task SeedAsync(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<User> userManager)
    {
        await SeedAsync(roleManager);
        await MigrateLegacyRoleAsync(roleManager, userManager, "User", RoleNames.Customer);
    }

    private static async Task MigrateLegacyRoleAsync(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<User> userManager,
        string legacyRoleName,
        string roleName)
    {
        var legacyRole = await roleManager.FindByNameAsync(legacyRoleName);
        if (legacyRole is null)
            return;

        var users = await userManager.GetUsersInRoleAsync(legacyRoleName);
        foreach (var user in users)
        {
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var addResult = await userManager.AddToRoleAsync(user, roleName);
                EnsureSucceeded(addResult, $"Could not migrate user '{user.Id}' to role '{roleName}'");
            }

            var removeResult = await userManager.RemoveFromRoleAsync(user, legacyRoleName);
            EnsureSucceeded(removeResult, $"Could not remove legacy role '{legacyRoleName}' from user '{user.Id}'");
        }

        var deleteResult = await roleManager.DeleteAsync(legacyRole);
        EnsureSucceeded(deleteResult, $"Could not delete legacy role '{legacyRoleName}'");
    }

    private static void EnsureSucceeded(IdentityResult result, string message)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException($"{message}: {string.Join(", ", result.Errors.Select(error => error.Description))}");
    }
}