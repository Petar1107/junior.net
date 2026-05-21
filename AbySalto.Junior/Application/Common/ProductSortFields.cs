using AbySalto.Junior.Domain.Entities;

namespace AbySalto.Junior.Application.Common;

public static class ProductSortFields
{
    public static readonly string Name = nameof(Product.Name);

    public static readonly string Price = nameof(Product.Price);

    public static readonly string UnitsInStock = nameof(Product.UnitsInStock);

    public static readonly string IsActive = nameof(Product.IsActive);

    public static readonly string CreatedOn = nameof(Product.CreatedOn);

    public static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        Name,
        Price,
        UnitsInStock,
        IsActive,
        CreatedOn,
    };

    public static string GetSortByOrDefault(string? sortBy) =>
        string.IsNullOrWhiteSpace(sortBy) ? Name : sortBy.Trim();

    public static bool Is(string sortBy, string field) =>
        string.Equals(sortBy, field, StringComparison.OrdinalIgnoreCase);
}
