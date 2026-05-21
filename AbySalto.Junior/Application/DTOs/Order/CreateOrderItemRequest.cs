namespace AbySalto.Junior.Application.DTOs.Order;

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? DiscountPercentage { get; set; }
}
