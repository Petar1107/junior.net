using AbySalto.Junior.Domain.Common;

namespace AbySalto.Junior.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }
    
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal DiscountPercentage { get; set; }

    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
