using AbySalto.Junior.Application.DTOs.Common;

namespace AbySalto.Junior.Application.DTOs.Product;

public class ProductResponse : BaseResponseDto
{
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public int UnitsInStock { get; init; }

    public bool IsActive { get; init; }
}
