namespace AbySalto.Junior.Application.DTOs.Order;

public class UpdateOrderItemRequest
{
    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? DiscountPercentage { get; set; }
}
