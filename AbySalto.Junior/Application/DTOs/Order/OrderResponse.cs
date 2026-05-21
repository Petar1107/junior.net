using AbySalto.Junior.Application.DTOs.Common;
using AbySalto.Junior.Domain.Enums;

namespace AbySalto.Junior.Application.DTOs.Order;

public class OrderResponse : BaseResponseDto
{
    public Guid? CustomerId { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public OrderStatus Status { get; init; }

    public DateTimeOffset OrderedAt { get; init; }

    public PaymentMethod PaymentMethod { get; init; }

    public string? DeliveryAddress { get; init; }

    public string ContactPhone { get; init; } = string.Empty;

    public string? Note { get; init; }

    public string Currency { get; init; } = "EUR";

    public decimal TotalAmount { get; init; }

    public IReadOnlyList<OrderItemResponse> Items { get; init; } = [];
}
