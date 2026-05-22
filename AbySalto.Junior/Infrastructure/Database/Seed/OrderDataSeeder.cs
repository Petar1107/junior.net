using AbySalto.Junior.Domain.Entities;
using AbySalto.Junior.Domain.Entities.Identity;
using AbySalto.Junior.Domain.Enums;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Junior.Infrastructure.Database.Seed;

internal static class OrderDataSeeder
{
    public const int OrderCount = 10;

    private const int DemoDataSeed = 20260521;

    private static readonly OrderStatus[] OrderStatuses =
    [
        OrderStatus.Pending,
        OrderStatus.Pending,
        OrderStatus.Pending,
        OrderStatus.Pending,
        OrderStatus.InPreparation,
        OrderStatus.InPreparation,
        OrderStatus.InPreparation,
        OrderStatus.Completed,
        OrderStatus.Completed,
        OrderStatus.Completed,
    ];

    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        if (await context.Orders.AnyAsync())
        {
            return;
        }

        var trackedProducts = await context.Products.ToListAsync();
        if (trackedProducts.Count == 0)
        {
            return;
        }

        var configuration = services.GetRequiredService<IConfiguration>();
        var seedSettings = configuration.GetSection("Seed").Get<SeedSettings>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? customer = null;
        if (!string.IsNullOrWhiteSpace(seedSettings?.CustomerEmail))
        {
            customer = await userManager.FindByEmailAsync(seedSettings.CustomerEmail);
        }

        Randomizer.Seed = new Random(DemoDataSeed);
        var faker = new Faker("hr");
        var orders = new List<Order>();

        for (var i = 0; i < OrderCount; i++)
        {
            var isRegisteredCustomer = customer is not null && faker.Random.Bool(0.55f);
            var order = BuildOrder(faker, isRegisteredCustomer, customer);
            order.Status = OrderStatuses[i];

            var itemCount = faker.Random.Int(1, Math.Min(4, trackedProducts.Count));
            var selectedProducts = faker.PickRandom(trackedProducts, itemCount).Distinct().ToList();

            foreach (var product in selectedProducts)
            {
                var quantity = faker.Random.Int(1, 3);
                product.UnitsInStock = Math.Max(0, product.UnitsInStock - quantity);

                order.Items.Add(new OrderItem
                {
                    Id = Guid.CreateVersion7(),
                    ProductId = product.Id,
                    UnitPrice = product.Price,
                    Quantity = quantity,
                    DiscountPercentage = faker.Random.Bool(0.35f)
                        ? decimal.Round(faker.Random.Decimal(5m, 15m), 2)
                        : 0m,
                });
            }

            orders.Add(order);
        }

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();
    }

    private static Order BuildOrder(
        Faker faker,
        bool isRegisteredCustomer,
        ApplicationUser? customer)
    {
        var orderedAt = DateTimeOffset.UtcNow.AddDays(-faker.Random.Int(0, 14))
            .AddHours(-faker.Random.Int(0, 12));

        if (isRegisteredCustomer && customer is not null)
        {
            return new Order
            {
                Id = Guid.CreateVersion7(),
                CustomerId = customer.Id,
                CustomerName = $"{customer.FirstName} {customer.LastName}".Trim(),
                OrderedAt = orderedAt,
                PaymentMethod = faker.PickRandom(PaymentMethod.Cash, PaymentMethod.Card, PaymentMethod.Online),
                ContactPhone = faker.Phone.PhoneNumber("+3859########"),
                DeliveryAddress = faker.Address.FullAddress(),
                Note = faker.Random.Bool(0.4f) ? faker.Lorem.Sentence() : null,
                Currency = "EUR",
                Status = OrderStatus.Pending,
            };
        }

        return new Order
        {
            Id = Guid.CreateVersion7(),
            CustomerName = faker.Name.FullName(),
            OrderedAt = orderedAt,
            PaymentMethod = faker.PickRandom(PaymentMethod.Cash, PaymentMethod.Card, PaymentMethod.DigitalWallet),
            ContactPhone = faker.Phone.PhoneNumber("+3859########"),
            DeliveryAddress = faker.Random.Bool(0.7f) ? faker.Address.FullAddress() : null,
            Note = faker.Random.Bool(0.3f) ? faker.Lorem.Sentence() : null,
            Currency = "EUR",
            Status = OrderStatus.Pending,
        };
    }
}
