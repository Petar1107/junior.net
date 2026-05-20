using AbySalto.Junior.Application.Services;
using AbySalto.Junior.Domain.Common;
using AbySalto.Junior.Domain.Entities;
using AbySalto.Junior.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Junior.Infrastructure.Database;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUser;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        ApplyAudit();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAudit();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAudit()
    {
        var auditUser = _currentUser.UserId?.ToString() ?? "system";
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = now;
                    entry.Entity.CreatedBy = auditUser;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedOn = now;
                    entry.Entity.UpdatedBy = auditUser;
                    entry.Property(entity => entity.CreatedOn).IsModified = false;
                    entry.Property(entity => entity.CreatedBy).IsModified = false;
                    break;
            }
        }
    }
}
