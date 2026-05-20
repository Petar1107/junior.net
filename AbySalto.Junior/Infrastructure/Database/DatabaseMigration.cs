using Microsoft.EntityFrameworkCore;

namespace AbySalto.Junior.Infrastructure.Database;

public static class DatabaseMigration
{
    public static async Task MigrateAndSeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var context = scopedServices.GetRequiredService<ApplicationDbContext>();
        var configuration = scopedServices.GetRequiredService<IConfiguration>();

        if (context.Database.IsNpgsql())
        {
            await context.Database.MigrateAsync();
        }

        await IdentityDataSeeder.SeedAsync(scopedServices, configuration);
    }
}
