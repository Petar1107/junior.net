namespace AbySalto.Junior.Infrastructure.Database.Seed;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            return;
        }

        await ProductDataSeeder.SeedAsync(services);
        await OrderDataSeeder.SeedAsync(services);
    }
}
