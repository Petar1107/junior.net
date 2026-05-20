using AbySalto.Junior.Domain.Common;

namespace AbySalto.Junior.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = String.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int UnitsInStock { get; set; }
    public bool IsActive { get; set; } = true;
}