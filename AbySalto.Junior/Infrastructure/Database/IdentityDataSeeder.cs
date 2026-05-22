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
        await SeedUserAsync(
            userManager,
            seedSettings.AdminEmail,
            seedSettings.AdminPassword,
            seedSettings.AdminFirstName,
            seedSettings.AdminLastName,
            UserRole.Admin);

        if (!string.IsNullOrWhiteSpace(seedSettings.CustomerEmail) &&
            !string.IsNullOrWhiteSpace(seedSettings.CustomerPassword))
        {
            await SeedUserAsync(
                userManager,
                seedSettings.CustomerEmail,
                seedSettings.CustomerPassword,
                seedSettings.CustomerFirstName,
                seedSettings.CustomerLastName,
                UserRole.Customer);
        }
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

    private static async Task SeedUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string firstName,
        string lastName,
        UserRole role)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
        };

        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create development user '{email}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
        }

        var roleResult = await userManager.AddToRoleAsync(user, UserRoleNames.From(role));
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to assign {role} role to '{email}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
        }
    }
}
