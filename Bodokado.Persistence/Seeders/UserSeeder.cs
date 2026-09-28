using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bodokado.Domain.Constants;
using Bodokado.Domain.Entities.Organizations;
using Bodokado.Domain.Entities.Users;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Seeders;

public static class UserSeeder
{
    public const string SeedPassword = "Seed@12345";
    public const string AdminUsername = "admin_demo";
    public const string AdminMobile = "09100000001";
    public const string CustomerUsernamePrefix = "customer_demo_";
    public const string CustomerMobilePrefix = "0910000";
    public const string UserOrganizationUsernamePrefix = "organization_demo_";
    public const string UserOrganizationMobilePrefix = "0910000";

    private record UserSeed(string Role, int Number, string Username, string Mobile, string FirstName, string LastName);

    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        CancellationToken ct = default)
    {
        var seeds = new List<UserSeed>
        {
            new(RoleNames.Admin, 1, AdminUsername, AdminMobile, "ادمین", "نمونه")
        };

        for (var number = 1; number <= 5; number++)
        {
            seeds.Add(new UserSeed(
                RoleNames.Customer,
                number,
                $"{CustomerUsernamePrefix}{number}",
                $"{CustomerMobilePrefix}{1000 + number:0000}",
                "مشتری",
                $"نمونه {number}"));
        }

        for (var number = 1; number <= 2; number++)
        {
            seeds.Add(new UserSeed(
                RoleNames.UserOrganization,
                number,
                $"{UserOrganizationUsernamePrefix}{number}",
                $"{UserOrganizationMobilePrefix}{2000 + number:0000}",
                "کاربر سازمانی",
                $"نمونه {number}"));
        }

        foreach (var seed in seeds)
        {
            var user = await userManager.FindByNameAsync(seed.Username)
                ?? await context.Users.FirstOrDefaultAsync(u => u.Mobile == seed.Mobile, ct);

            if (user is null)
            {
                user = new User
                {
                    Id = DeterministicGuid.Create($"SeedUser_{seed.Role}_{seed.Number}"),
                    UserName = seed.Username,
                    Mobile = seed.Mobile,
                    PhoneNumber = seed.Mobile,
                    PhoneNumberConfirmed = true,
                    FirstName = seed.FirstName,
                    LastName = seed.LastName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(user, SeedPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(error => error.Description));
                    throw new InvalidOperationException($"Could not create seed user '{seed.Username}': {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, seed.Role))
            {
                var roleResult = await userManager.AddToRoleAsync(user, seed.Role);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(error => error.Description));
                    throw new InvalidOperationException($"Could not add '{seed.Username}' to role '{seed.Role}': {errors}");
                }
            }

            if (seed.Role == RoleNames.UserOrganization)
                await EnsureOrganizationMembershipAsync(context, user, seed.Number, ct);
        }
    }

    private static async Task EnsureOrganizationMembershipAsync(
        AppDbContext context,
        User user,
        int number,
        CancellationToken ct)
    {
        if (await context.UserOrganizations.AnyAsync(membership => membership.UserId == user.Id, ct))
            return;

        var organizationId = DeterministicGuid.Create($"SeedOrganization_user_organization_{number}");
        var organization = await context.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId, ct);
        if (organization is null)
        {
            organization = new Organization
            {
                Id = organizationId,
                OrganizationName = $"سازمان نمونه {number}",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Organizations.Add(organization);
        }

        context.UserOrganizations.Add(new UserOrganization
        {
            UserId = user.Id,
            OrganizationId = organizationId,
            IsActive = true
        });

        await context.SaveChangesAsync(ct);
    }
}