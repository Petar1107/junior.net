using AbySalto.Junior.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Junior.Infrastructure.Database;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
