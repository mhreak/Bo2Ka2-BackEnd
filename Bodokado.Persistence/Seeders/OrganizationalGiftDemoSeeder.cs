using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bodokado.Domain.Constants;
using Bodokado.Domain.Entities.Organizations;
using Bodokado.Domain.Entities.Users;
using Bodokado.Domain.Enums;
using Bodokado.Persistence.Context;

namespace Bodokado.Persistence.Seeders;

public static class OrganizationalGiftDemoSeeder
{
    public const string DemoOrgName = "بانک ملت";
    public const string SecondOrgName = "سازمان دمو";
    public const string OrgAdminUsername = "org_admin_demo_1";
    public const string OrgAdminMobile = "09100003001";
    public const string SeedPassword = "Seed@12345";

    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        CancellationToken ct = default)
    {
        // 1) Ensure AdminOrganization role exists
        if (!await roleManager.RoleExistsAsync(RoleNames.AdminOrganization))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = RoleNames.AdminOrganization });
        }

        // 2) Seed Organization(s)
        var org1Id = DeterministicGuid.Create("SeedOrg_BankMellat");
        var org1 = await context.Organizations
            .FirstOrDefaultAsync(o => o.Id == org1Id || o.OrganizationName == DemoOrgName, ct);

        if (org1 is null)
        {
            org1 = new Organization
            {
                Id = org1Id,
                OrganizationName = DemoOrgName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Organizations.Add(org1);
            await context.SaveChangesAsync(ct);
        }

        var org2Id = DeterministicGuid.Create("SeedOrg_DemoOrganization");
        var org2 = await context.Organizations
            .FirstOrDefaultAsync(o => o.Id == org2Id || o.OrganizationName == SecondOrgName, ct);

        if (org2 is null)
        {
            org2 = new Organization
            {
                Id = org2Id,
                OrganizationName = SecondOrgName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Organizations.Add(org2);
            await context.SaveChangesAsync(ct);
        }

        // 3) AdminOrganization user linked via User.OrganizationId
        var orgAdminUser = await userManager.FindByNameAsync(OrgAdminUsername)
            ?? await context.Users.FirstOrDefaultAsync(u => u.Mobile == OrgAdminMobile, ct);

        if (orgAdminUser is null)
        {
            orgAdminUser = new User
            {
                Id = DeterministicGuid.Create("SeedUser_AdminOrganization_1"),
                UserName = OrgAdminUsername,
                Mobile = OrgAdminMobile,
                PhoneNumber = OrgAdminMobile,
                PhoneNumberConfirmed = true,
                FirstName = "مدیر سازمان",
                LastName = "بانک ملت",
                OrganizationId = org1.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createRes = await userManager.CreateAsync(orgAdminUser, SeedPassword);
            if (!createRes.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create seed user {OrgAdminUsername}: " +
                    string.Join(", ", createRes.Errors.Select(e => e.Description)));
            }
        }
        else if (orgAdminUser.OrganizationId != org1.Id)
        {
            orgAdminUser.OrganizationId = org1.Id;
            await userManager.UpdateAsync(orgAdminUser);
        }

        if (!await userManager.IsInRoleAsync(orgAdminUser, RoleNames.AdminOrganization))
        {
            await userManager.AddToRoleAsync(orgAdminUser, RoleNames.AdminOrganization);
        }

        // 4) OrganizationPersonnelCategories for demo org
        var managerCatId = DeterministicGuid.Create($"SeedPersonnelCat_{org1.Id}_Manager");
        var managerCat = await context.OrganizationPersonnelCategories
            .FirstOrDefaultAsync(c => c.Id == managerCatId || (c.OrganizationId == org1.Id && c.CategoryName == "مدیر"), ct);

        if (managerCat is null)
        {
            managerCat = new OrganizationPersonnelCategory
            {
                Id = managerCatId,
                OrganizationId = org1.Id,
                CategoryName = "مدیر",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.OrganizationPersonnelCategories.Add(managerCat);
            await context.SaveChangesAsync(ct);
        }

        var staffCatId = DeterministicGuid.Create($"SeedPersonnelCat_{org1.Id}_Staff");
        var staffCat = await context.OrganizationPersonnelCategories
            .FirstOrDefaultAsync(c => c.Id == staffCatId || (c.OrganizationId == org1.Id && c.CategoryName == "کارمند"), ct);

        if (staffCat is null)
        {
            staffCat = new OrganizationPersonnelCategory
            {
                Id = staffCatId,
                OrganizationId = org1.Id,
                CategoryName = "کارمند",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.OrganizationPersonnelCategories.Add(staffCat);
            await context.SaveChangesAsync(ct);
        }

        // 5) OrganizationalGiftCampaign
        var campaignId = DeterministicGuid.Create($"SeedGiftCampaign_{org1.Id}_Yalda");
        var campaign = await context.OrganizationalGiftCampaigns
            .Include(c => c.Constraints)
            .FirstOrDefaultAsync(c => c.Id == campaignId, ct);

        if (campaign is null)
        {
            var now = DateTime.UtcNow;
            campaign = new OrganizationalGiftCampaign
            {
                Id = campaignId,
                OrganizationId = org1.Id,
                CampaignName = "کمپین شب یلدا ۱۴۰۵",
                OccasionType = OccasionType.YaldaNight,
                StartDateTime = now.AddDays(-1),
                FinishDateTime = now.AddDays(30),
                IsActiveByAdmin = true,
                OrganizationalMessageEnabled = true,
                MessageType = OrganizationalMessageType.Text,
                OrganizationalMessage = "یلداتون مبارک! هدیه‌ای ویژه از طرف بانک ملت برای شما.",
                CreatedAt = now
            };

            // Constraint 1: General PriceLimit without category (3,000,000)
            campaign.Constraints.Add(new OrganizationalGiftCampaignConstraint
            {
                Id = DeterministicGuid.Create($"SeedConstraint_{campaignId}_PriceDefault"),
                OrganizationalGiftCampaignId = campaignId,
                ConstraintType = GiftCampaignConstraintType.PriceLimit,
                OrganizationPersonnelCategoryId = null,
                Constraint = "3000000",
                CreatedAt = now
            });

            // Constraint 2: PriceLimit for Managers (10,000,000)
            campaign.Constraints.Add(new OrganizationalGiftCampaignConstraint
            {
                Id = DeterministicGuid.Create($"SeedConstraint_{campaignId}_PriceManager"),
                OrganizationalGiftCampaignId = campaignId,
                ConstraintType = GiftCampaignConstraintType.PriceLimit,
                OrganizationPersonnelCategoryId = managerCat.Id,
                Constraint = "10000000",
                CreatedAt = now
            });

            // Optional: ShopLimit if demo shops exist in DB
            var demoShops = await context.Shops
                .Where(s => !s.IsDeleted)
                .Take(2)
                .Select(s => s.Id)
                .ToListAsync(ct);

            if (demoShops.Count > 0)
            {
                campaign.Constraints.Add(new OrganizationalGiftCampaignConstraint
                {
                    Id = DeterministicGuid.Create($"SeedConstraint_{campaignId}_ShopLimit"),
                    OrganizationalGiftCampaignId = campaignId,
                    ConstraintType = GiftCampaignConstraintType.ShopLimit,
                    OrganizationPersonnelCategoryId = null,
                    Constraint = string.Join(",", demoShops),
                    CreatedAt = now
                });
            }

            // Optional: CategoryLimit if product categories exist in DB
            var demoCategories = await context.ProductCategories
                .Where(c => !c.IsDeleted)
                .Take(2)
                .Select(c => c.Id)
                .ToListAsync(ct);

            if (demoCategories.Count > 0)
            {
                campaign.Constraints.Add(new OrganizationalGiftCampaignConstraint
                {
                    Id = DeterministicGuid.Create($"SeedConstraint_{campaignId}_CategoryLimit"),
                    OrganizationalGiftCampaignId = campaignId,
                    ConstraintType = GiftCampaignConstraintType.CategoryLimit,
                    OrganizationPersonnelCategoryId = null,
                    Constraint = string.Join(",", demoCategories),
                    CreatedAt = now
                });
            }

            context.OrganizationalGiftCampaigns.Add(campaign);
            await context.SaveChangesAsync(ct);
        }

        // 6) Gift codes (UserOrganizationalGiftCampaign)
        var codesToSeed = new[]
        {
            new { Code = "DEMOGIFT01", IsUsed = false },
            new { Code = "DEMOGIFT02", IsUsed = false },
            new { Code = "DEMOGIFT03", IsUsed = false },
            new { Code = "DEMOUSED01", IsUsed = true }
        };

        var firstCustomer = await userManager.FindByNameAsync($"{UserSeeder.CustomerUsernamePrefix}1");

        foreach (var item in codesToSeed)
        {
            var exists = await context.UserOrganizationalGiftCampaigns
                .AnyAsync(x => x.GiftCode == item.Code, ct);

            if (!exists)
            {
                var now = DateTime.UtcNow;
                var entity = new UserOrganizationalGiftCampaign
                {
                    Id = DeterministicGuid.Create($"SeedGiftCode_{item.Code}"),
                    OrganizationalGiftCampaignId = campaign.Id,
                    GiftCode = item.Code,
                    CreatedAt = now
                };

                if (item.IsUsed && firstCustomer is not null)
                {
                    entity.UserId = firstCustomer.Id;
                    entity.UsedAt = now;
                    entity.UpdatedAt = now;
                }
                else
                {
                    entity.UserId = null;
                    entity.UsedAt = null;
                }

                context.UserOrganizationalGiftCampaigns.Add(entity);
            }
        }

        await context.SaveChangesAsync(ct);
    }
}
