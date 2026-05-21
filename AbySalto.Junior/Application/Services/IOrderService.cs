using AbySalto.Junior.Application.DTOs.Common;
using AbySalto.Junior.Application.DTOs.Order;

namespace AbySalto.Junior.Application.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResponse<OrderListItemResponse>> GetAllAsync(
        OrderListQuery query,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> UpdateAsync(
        Guid id,
        UpdateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> AddItemAsync(
        Guid orderId,
        CreateOrderItemRequest request,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> UpdateItemAsync(
        Guid orderId,
        Guid itemId,
        UpdateOrderItemRequest request,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> RemoveItemAsync(
        Guid orderId,
        Guid itemId,
        CancellationToken cancellationToken = default);
}
