using AbySalto.Junior.Application.DTOs.Common;

namespace AbySalto.Junior.Application.DTOs.Order;

public class OrderItemResponse : BaseResponseDto
{
    public Guid ProductId { get; init; }

    public string ProductName { get; init; } = string.Empty;

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal DiscountPercentage { get; init; }

    public decimal ItemTotal { get; init; }
}
