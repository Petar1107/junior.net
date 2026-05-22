using AbySalto.Junior.Application.Common;
using AbySalto.Junior.Domain.Entities;
using FluentAssertions;

namespace AbySalto.Junior.UnitTests.Common;

public class OrderCalculationsTests
{
    [Fact]
    public void CalculateItemTotal_WithoutDiscount_ReturnsUnitPriceTimesQuantity()
    {
        var total = OrderCalculations.CalculateItemTotal(10m, 3, 0m);

        total.Should().Be(30m);
    }

    [Fact]
    public void CalculateItemTotal_WithDiscount_AppliesPercentage()
    {
        var total = OrderCalculations.CalculateItemTotal(100m, 2, 10m);

        total.Should().Be(180m);
    }

    [Fact]
    public void CalculateOrderTotal_SumsAllItems()
    {
        var items = new[]
        {
            new OrderItem { UnitPrice = 10m, Quantity = 2, DiscountPercentage = 0m },
            new OrderItem { UnitPrice = 5m, Quantity = 4, DiscountPercentage = 0m },
        };

        var total = OrderCalculations.CalculateOrderTotal(items);

        total.Should().Be(40m);
    }

    [Fact]
    public void CalculateItemTotal_WithFullDiscount_ReturnsZero()
    {
        var total = OrderCalculations.CalculateItemTotal(50m, 2, 100m);

        total.Should().Be(0m);
    }

    [Fact]
    public void CalculateItemTotal_FromOrderItem_uses_entity_values()
    {
        var item = new OrderItem
        {
            UnitPrice = 25m,
            Quantity = 2,
            DiscountPercentage = 20m,
        };

        var total = OrderCalculations.CalculateItemTotal(item);

        total.Should().Be(40m);
    }
}
