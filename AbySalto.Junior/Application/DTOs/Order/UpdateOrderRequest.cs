using AbySalto.Junior.Domain.Enums;

namespace AbySalto.Junior.Application.DTOs.Order;

public class UpdateOrderRequest
{
    public OrderStatus? Status { get; set; }

    public string? CustomerName { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }

    public string? ContactPhone { get; set; }

    public string? DeliveryAddress { get; set; }

    public string? Note { get; set; }

    public string? Currency { get; set; }
}
