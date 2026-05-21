using AbySalto.Junior.Application.DTOs.Common;
using AbySalto.Junior.Domain.Enums;

namespace AbySalto.Junior.Application.DTOs.Order;

public class OrderListQuery : PaginationQuery
{
    public OrderStatus? Status { get; init; }

    public string? SortBy { get; init; }

    public SortDirection? SortDirection { get; init; }
}
