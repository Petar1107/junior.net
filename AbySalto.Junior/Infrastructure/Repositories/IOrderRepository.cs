using AbySalto.Junior.Domain.Entities;

namespace AbySalto.Junior.Infrastructure.Repositories;

public interface IOrderRepository
{
    IQueryable<Order> Query();

    Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    void RemoveItem(OrderItem item);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
