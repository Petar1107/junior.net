using System.Net;
using System.Net.Http.Json;
using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Application.DTOs.Product;
using AbySalto.Junior.Domain.Enums;
using AbySalto.Junior.IntegrationTests.Fixtures;
using FluentAssertions;

namespace AbySalto.Junior.IntegrationTests.Endpoints;

[Collection(nameof(IntegrationTestCollection))]
public class OrdersApiTests
{
    private readonly HttpClient _client;

    public OrdersApiTests(AbySaltoApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_WithProductLine_calculatesTotalAndReturnsCreated()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var token = await IntegrationTestHelper.LoginAsAdminAsync(_client, cancellationToken);
        IntegrationTestHelper.SetBearerToken(_client, token);

        var productName = $"Test Pizza {Guid.NewGuid():N}";
        var productResponse = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest
            {
                Name = productName,
                Price = 12.50m,
                UnitsInStock = 20,
                IsActive = true,
            },
            cancellationToken);

        productResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>(
            IntegrationTestHelper.JsonOptions,
            cancellationToken);
        product.Should().NotBeNull();

        var orderResponse = await _client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest
            {
                CustomerName = "Integration Guest",
                PaymentMethod = PaymentMethod.Card,
                ContactPhone = "+385911234567",
                Currency = "EUR",
                Items =
                [
                    new CreateOrderItemRequest
                    {
                        ProductId = product.Id,
                        Quantity = 2,
                        DiscountPercentage = 10m,
                    },
                ],
            },
            cancellationToken);

        orderResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var order = await orderResponse.Content.ReadFromJsonAsync<OrderResponse>(
            IntegrationTestHelper.JsonOptions,
            cancellationToken);
        order.Should().NotBeNull();
        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().ContainSingle();
        order.TotalAmount.Should().Be(22.50m);
    }

    [Fact]
    public async Task UpdateOrderStatus_AsAdmin_FromPendingToInPreparation_ReturnsOk()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var token = await IntegrationTestHelper.LoginAsAdminAsync(_client, cancellationToken);
        IntegrationTestHelper.SetBearerToken(_client, token);

        var productResponse = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest
            {
                Name = $"Status Test {Guid.NewGuid():N}",
                Price = 10m,
                UnitsInStock = 10,
                IsActive = true,
            },
            cancellationToken);
        productResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>(
            IntegrationTestHelper.JsonOptions,
            cancellationToken);

        var createResponse = await _client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest
            {
                CustomerName = "Status Guest",
                PaymentMethod = PaymentMethod.Cash,
                ContactPhone = "+385911234567",
                Currency = "EUR",
                Items =
                [
                    new CreateOrderItemRequest
                    {
                        ProductId = product!.Id,
                        Quantity = 1,
                    },
                ],
            },
            cancellationToken);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderResponse>(
            IntegrationTestHelper.JsonOptions,
            cancellationToken);
        createdOrder!.Status.Should().Be(OrderStatus.Pending);

        var updateResponse = await _client.PatchAsJsonAsync(
            $"/api/orders/{createdOrder.Id}",
            new UpdateOrderRequest { Status = OrderStatus.InPreparation },
            IntegrationTestHelper.JsonOptions,
            cancellationToken);

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedOrder = await updateResponse.Content.ReadFromJsonAsync<OrderResponse>(
            IntegrationTestHelper.JsonOptions,
            cancellationToken);
        updatedOrder.Should().NotBeNull();
        updatedOrder.Status.Should().Be(OrderStatus.InPreparation);
    }
}
