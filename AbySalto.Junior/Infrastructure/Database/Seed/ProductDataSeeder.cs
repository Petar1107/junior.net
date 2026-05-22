using AbySalto.Junior.Domain.Entities;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Junior.Infrastructure.Database.Seed;

internal static class ProductDataSeeder
{
    public const int ProductCount = 10;

    private const int DemoDataSeed = 20260520;

    private static readonly string[] MenuItems =
    [
        "Margherita Pizza",
        "Carbonara",
        "Caesar Salad",
        "Tomato Soup",
        "Tiramisu",
        "Grilled Pljeskavica",
        "Grilled Fish",
        "Cheeseburger",
        "Mushroom Risotto",
        "Bruschetta",
    ];

    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        if (await context.Products.AnyAsync())
        {
            return;
        }

        Randomizer.Seed = new Random(DemoDataSeed);
        var faker = new Faker("hr");

        var products = MenuItems
            .Take(ProductCount)
            .Select((name, index) => new Product
            {
                Id = Guid.CreateVersion7(),
                Name = name,
                Description = faker.Lorem.Sentence(6 + index % 4),
                Price = decimal.Round(faker.Random.Decimal(5m, 28m), 2),
                UnitsInStock = faker.Random.Int(80, 150),
                IsActive = faker.Random.Bool(0.9f),
            })
            .ToList();

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
