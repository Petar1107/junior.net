using AbySalto.Junior.Application.Common;
using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Domain.Enums;
using FluentAssertions;

namespace AbySalto.Junior.UnitTests.Common;

public class OrderRulesTests
{
    [Theory]
    [InlineData(OrderStatus.Pending, OrderStatus.InPreparation, true)]
    [InlineData(OrderStatus.InPreparation, OrderStatus.Completed, true)]
    [InlineData(OrderStatus.Pending, OrderStatus.Completed, false)]
    [InlineData(OrderStatus.Pending, OrderStatus.Pending, false)]
    [InlineData(OrderStatus.InPreparation, OrderStatus.Pending, false)]
    [InlineData(OrderStatus.Completed, OrderStatus.InPreparation, false)]
    public void IsValidStatusTransition_ReturnsExpected(
        OrderStatus current,
        OrderStatus next,
        bool expected)
    {
        OrderRules.IsValidStatusTransition(current, next).Should().Be(expected);
    }

    [Fact]
    public void HasDuplicateProducts_WhenSameProductTwice_ReturnsTrue()
    {
        var productId = Guid.NewGuid();
        var items = new[]
        {
            new CreateOrderItemRequest { ProductId = productId, Quantity = 1 },
            new CreateOrderItemRequest { ProductId = productId, Quantity = 2 },
        };

        OrderRules.HasDuplicateProducts(items).Should().BeTrue();
    }

    [Fact]
    public void HasDuplicateProducts_WhenAllProductsUnique_ReturnsFalse()
    {
        var items = new[]
        {
            new CreateOrderItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 },
            new CreateOrderItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 },
        };

        OrderRules.HasDuplicateProducts(items).Should().BeFalse();
    }
}
