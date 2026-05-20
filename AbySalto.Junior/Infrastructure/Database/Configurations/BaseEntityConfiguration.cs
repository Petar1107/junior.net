using AbySalto.Junior.Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbySalto.Junior.Infrastructure.Database.Configurations;

public static class BaseEntityConfiguration
{
    public static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder.HasKey(entity => entity.Id);
        
        builder.Property(entity => entity.Id)
            .ValueGeneratedNever();
        
        builder.Property(entity => entity.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(entity => entity.CreatedOn)
            .IsRequired();

        builder.Property(entity => entity.UpdatedBy)
            .HasMaxLength(256);

        builder.Property(entity => entity.UpdatedOn);
    }
}
