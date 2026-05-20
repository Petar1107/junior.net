using AbySalto.Junior.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbySalto.Junior.Infrastructure.Database.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ConfigureBaseEntity();

        builder.Property(order => order.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(order => order.OrderedAt)
            .IsRequired();

        builder.Property(order => order.DeliveryAddress)
            .HasMaxLength(500);

        builder.Property(order => order.ContactPhone)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(order => order.Note)
            .HasMaxLength(1000);

        builder.Property(order => order.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.HasIndex(order => order.CustomerId);

        builder.HasIndex(order => order.OrderedAt);

        builder.HasOne(order => order.Customer)
            .WithMany()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(order => order.Items)
            .WithOne(item => item.Order)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
