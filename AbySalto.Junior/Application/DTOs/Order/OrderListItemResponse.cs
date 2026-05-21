using AbySalto.Junior.Domain.Enums;

namespace AbySalto.Junior.Application.DTOs.Order;

public class OrderListItemResponse
{
    public Guid Id { get; init; }

    public Guid? CustomerId { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public OrderStatus Status { get; init; }

    public DateTimeOffset OrderedAt { get; init; }

    public PaymentMethod PaymentMethod { get; init; }

    public decimal TotalAmount { get; init; }
}
