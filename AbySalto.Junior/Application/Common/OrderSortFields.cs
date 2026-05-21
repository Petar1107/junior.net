using AbySalto.Junior.Domain.Entities;

namespace AbySalto.Junior.Application.Common;

public static class OrderSortFields
{
    public const string TotalAmount = "TotalAmount";

    public static readonly string OrderedAt = nameof(Order.OrderedAt);

    public static readonly string Status = nameof(Order.Status);

    public static readonly string CustomerName = nameof(Order.CustomerName);

    public static readonly string CreatedOn = nameof(Order.CreatedOn);

    public static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        TotalAmount,
        OrderedAt,
        Status,
        CustomerName,
        CreatedOn,
    };

    public static string GetSortByOrDefault(string? sortBy) =>
        string.IsNullOrWhiteSpace(sortBy) ? OrderedAt : sortBy.Trim();

    public static bool Is(string sortBy, string field) =>
        string.Equals(sortBy, field, StringComparison.OrdinalIgnoreCase);
}
