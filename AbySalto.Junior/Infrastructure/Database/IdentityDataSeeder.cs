using AbySalto.Junior.Domain.Entities.Identity;
using AbySalto.Junior.Domain.Enums;
using AbySalto.Junior.Infrastructure.Database.Seed;
using Microsoft.AspNetCore.Identity;

namespace AbySalto.Junior.Infrastructure.Database;

public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await SeedRolesAsync(roleManager);

        var seedSettings = configuration.GetSection("Seed").Get<SeedSettings>();
        if (seedSettings is null ||
            string.IsNullOrWhiteSpace(seedSettings.AdminEmail) ||
            string.IsNullOrWhiteSpace(seedSettings.AdminPassword))
        {
            return;
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await SeedAdminUserAsync(userManager, seedSettings);
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        foreach (var roleName in UserRoleNames.All())
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        SeedSettings seedSettings)
    {
        var existingUser = await userManager.FindByEmailAsync(seedSettings.AdminEmail);
        if (existingUser is not null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = seedSettings.AdminEmail,
            Email = seedSettings.AdminEmail,
            EmailConfirmed = true,
            FirstName = seedSettings.AdminFirstName,
            LastName = seedSettings.AdminLastName,
        };

        var createResult = await userManager.CreateAsync(admin, seedSettings.AdminPassword);
        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create development admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
        }

        var roleResult = await userManager.AddToRoleAsync(admin, UserRoleNames.From(UserRole.Admin));
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to assign Admin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
        }
    }
}
