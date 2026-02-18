using EnterpriseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Order entity.
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.PaymentStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.Subtotal)
            .HasPrecision(18, 4);

        builder.Property(o => o.DiscountAmount)
            .HasPrecision(18, 4);

        builder.Property(o => o.TaxAmount)
            .HasPrecision(18, 4);

        builder.Property(o => o.ShippingAmount)
            .HasPrecision(18, 4);

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 4);

        builder.Property(o => o.Currency)
            .HasMaxLength(3);

        builder.Property(o => o.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(o => o.PaymentReference)
            .HasMaxLength(100);

        // Shipping address
        builder.Property(o => o.ShippingStreet).HasMaxLength(200);
        builder.Property(o => o.ShippingCity).HasMaxLength(100);
        builder.Property(o => o.ShippingState).HasMaxLength(100);
        builder.Property(o => o.ShippingPostalCode).HasMaxLength(20);
        builder.Property(o => o.ShippingCountry).HasMaxLength(100);

        // Billing address
        builder.Property(o => o.BillingStreet).HasMaxLength(200);
        builder.Property(o => o.BillingCity).HasMaxLength(100);
        builder.Property(o => o.BillingState).HasMaxLength(100);
        builder.Property(o => o.BillingPostalCode).HasMaxLength(20);
        builder.Property(o => o.BillingCountry).HasMaxLength(100);

        builder.Property(o => o.Notes)
            .HasMaxLength(2000);

        builder.Property(o => o.CustomerNotes)
            .HasMaxLength(1000);

        builder.Property(o => o.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(o => o.ShippingCarrier)
            .HasMaxLength(50);

        // Relationships
        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.HasIndex(o => o.CustomerId);

        builder.HasIndex(o => o.Status);

        builder.HasIndex(o => o.PaymentStatus);

        builder.HasIndex(o => o.OrderDate);

        builder.HasIndex(o => o.CreatedAt);

        // Global query filter for soft delete
        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}
