using EnterpriseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for StockMovement entity.
/// </summary>
public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.ReferenceNumber)
            .HasMaxLength(50);

        builder.Property(s => s.Reason)
            .HasMaxLength(200);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.Property(s => s.UnitCost)
            .HasPrecision(18, 4);

        builder.Property(s => s.CreatedByUsername)
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.ProductVariant)
            .WithMany()
            .HasForeignKey(s => s.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(s => s.ProductId);
        builder.HasIndex(s => s.ProductVariantId);
        builder.HasIndex(s => s.Type);
        builder.HasIndex(s => s.ReferenceNumber);
        builder.HasIndex(s => s.CreatedAt);
    }
}
