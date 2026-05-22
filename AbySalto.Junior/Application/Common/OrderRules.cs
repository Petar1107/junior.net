using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Domain.Enums;

namespace AbySalto.Junior.Application.Common;

public static class OrderRules
{
    public static bool IsValidStatusTransition(OrderStatus current, OrderStatus next) =>
        (current, next) is
        (OrderStatus.Pending, OrderStatus.InPreparation)
        or (OrderStatus.InPreparation, OrderStatus.Completed);

    public static bool HasDuplicateProducts(IEnumerable<CreateOrderItemRequest> items) =>
        items.GroupBy(item => item.ProductId).Any(group => group.Count() > 1);
}
