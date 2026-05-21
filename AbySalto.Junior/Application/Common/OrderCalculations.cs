using AbySalto.Junior.Domain.Entities;

namespace AbySalto.Junior.Application.Common;

public static class OrderCalculations
{
    public static decimal CalculateItemTotal(
        decimal unitPrice,
        int quantity,
        decimal discountPercentage) =>
        unitPrice * quantity * (1 - discountPercentage / 100m);

    public static decimal CalculateItemTotal(OrderItem item) =>
        CalculateItemTotal(item.UnitPrice, item.Quantity, item.DiscountPercentage);

    public static decimal CalculateOrderTotal(IEnumerable<OrderItem> items) =>
        items.Sum(CalculateItemTotal);
}
