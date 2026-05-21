using AbySalto.Junior.Domain.Enums;

namespace AbySalto.Junior.Application.DTOs.Order;

public class CreateOrderRequest
{
    public Guid? CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public string ContactPhone { get; set; } = string.Empty;

    public string? DeliveryAddress { get; set; }

    public string? Note { get; set; }

    public string Currency { get; set; } = "EUR";

    public IList<CreateOrderItemRequest> Items { get; set; } = [];
}
