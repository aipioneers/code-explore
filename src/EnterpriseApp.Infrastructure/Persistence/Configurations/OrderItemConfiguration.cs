using EnterpriseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for OrderItem entity.
/// </summary>
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.ProductSku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.VariantName)
            .HasMaxLength(200);

        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 4);

        builder.Property(i => i.UnitDiscount)
            .HasPrecision(18, 4);

        builder.Property(i => i.TaxRate)
            .HasPrecision(5, 2);

        builder.Property(i => i.LineTotal)
            .HasPrecision(18, 4);

        builder.Property(i => i.TaxAmount)
            .HasPrecision(18, 4);

        builder.Property(i => i.LineTotalWithTax)
            .HasPrecision(18, 4);

        builder.Property(i => i.Notes)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(i => i.OrderId);

        builder.HasIndex(i => i.ProductId);
    }
}
