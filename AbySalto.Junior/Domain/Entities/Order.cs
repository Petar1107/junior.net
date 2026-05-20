using AbySalto.Junior.Domain.Common;
using AbySalto.Junior.Domain.Entities.Identity;
using AbySalto.Junior.Domain.Enums;

namespace AbySalto.Junior.Domain.Entities;

public class Order : BaseEntity
{
    public Guid? CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTimeOffset OrderedAt { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public string? DeliveryAddress { get; set; }

    public string ContactPhone { get; set; } = string.Empty;

    public string? Note { get; set; }

    public string Currency { get; set; } = "EUR";
    
    public ApplicationUser? Customer { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
