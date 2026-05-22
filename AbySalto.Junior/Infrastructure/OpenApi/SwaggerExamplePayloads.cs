using AbySalto.Junior.Domain.Enums;
using AbySalto.Junior.Infrastructure.Database.Seed;

namespace AbySalto.Junior.Infrastructure.OpenApi;

internal static class SwaggerExamplePayloads
{
    public static object SeededAdminLogin(SeedSettings? settings) => new
    {
        email = string.IsNullOrWhiteSpace(settings?.AdminEmail) ? "admin@example.com" : settings.AdminEmail,
        password = string.IsNullOrWhiteSpace(settings?.AdminPassword) ? "Admin123!" : settings.AdminPassword,
    };

    public static object Register => new
    {
        email = "newuser@example.com",
        password = "Register123!",
        firstName = "New",
        lastName = "User",
    };

    public const string PlaceholderGuidNote =
        "Sample IDs are placeholders. Use product ids from GET /api/products and customer ids from registered users.";

    public static readonly Guid SampleProductId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid SampleCustomerId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static object CreateProduct => new
    {
        name = "Margherita Pizza",
        description = "Tomato sauce, mozzarella, fresh basil",
        price = 12.50m,
        unitsInStock = 40,
        isActive = true,
    };

    public static object UpdateProduct => new
    {
        name = "Margherita Pizza (large)",
        description = "32 cm, extra mozzarella",
        price = 15.00m,
        unitsInStock = 35,
        isActive = true,
    };

    public static object GuestCreateOrder => new
    {
        customerName = "Ivan Horvat",
        paymentMethod = PaymentMethod.Card,
        contactPhone = "+385911234567",
        deliveryAddress = "Velebitska ul. 1, 21000 Split",
        note = "No onions",
        currency = "EUR",
        items = new[]
        {
            new { productId = SampleProductId, quantity = 2 },
        },
    };

    public static object RegisteredCustomerCreateOrder => new
    {
        customerId = SampleCustomerId,
        paymentMethod = PaymentMethod.Online,
        contactPhone = "+385989876543",
        deliveryAddress = "Vukovarska ul. 10, 21000 Split",
        note = "Ring the bell",
        currency = "EUR",
        items = new[]
        {
            new { productId = SampleProductId, quantity = 1, discountPercentage = 10m },
        },
    };

    public static object UpdateOrderHeader => new
    {
        customerName = "Ivan Horvat (updated)",
        paymentMethod = PaymentMethod.Cash,
        contactPhone = "+385911234567",
        deliveryAddress = "Velebitska ul. 1, 21000 Split",
        note = "Extra napkins",
        currency = "EUR",
    };

    public static object UpdateOrderStatus => new
    {
        status = OrderStatus.InPreparation,
    };

    public static object AddOrderItem => new
    {
        productId = SampleProductId,
        quantity = 1,
        discountPercentage = 5m,
    };

    public static object UpdateOrderItem => new
    {
        quantity = 3,
        discountPercentage = 0m,
    };
}
