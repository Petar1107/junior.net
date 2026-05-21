using AbySalto.Junior.Application.DTOs.Common;

namespace AbySalto.Junior.Application.DTOs.Product;

public class ProductListQuery : PaginationQuery
{
    public string? Search { get; init; }

    public bool? IsActive { get; init; }

    public string? SortBy { get; init; }

    public SortDirection? SortDirection { get; init; }
}
