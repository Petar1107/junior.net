using AbySalto.Junior.Infrastructure.Database.Seed;
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
        var environment = scopedServices.GetRequiredService<IHostEnvironment>();

        if (context.Database.IsNpgsql())
        {
            await context.Database.MigrateAsync();
        }

        await IdentityDataSeeder.SeedAsync(scopedServices, configuration);

        if (environment.IsDevelopment())
        {
            await DevelopmentDataSeeder.SeedAsync(scopedServices, environment);
        }
    }
}
