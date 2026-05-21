using AbySalto.Junior.Domain.Entities;

namespace AbySalto.Junior.Infrastructure.Repositories;

public interface IProductRepository
{
    IQueryable<Product> Query();

    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    void Remove(Product product);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
