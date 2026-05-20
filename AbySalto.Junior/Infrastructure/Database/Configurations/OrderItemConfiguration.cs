using AbySalto.Junior.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbySalto.Junior.Infrastructure.Database.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ConfigureBaseEntity();

        builder.Property(item => item.ProductId)
            .IsRequired();
        
        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(item => item.DiscountPercentage)
            .HasPrecision(5, 2)
            .HasDefaultValue(0m);

        builder.HasIndex(item => item.OrderId);

        builder.HasIndex(item => item.ProductId);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_OrderItems_Quantity",
                "\"Quantity\" >= 1");
            table.HasCheckConstraint(
                "CK_OrderItems_DiscountPercentage",
                "\"DiscountPercentage\" >= 0 AND \"DiscountPercentage\" <= 100");
        });

        builder.HasOne(item => item.Product)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
