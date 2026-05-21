namespace AbySalto.Junior.Application.DTOs.Product;

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int UnitsInStock { get; set; }

    public bool IsActive { get; set; }
}
