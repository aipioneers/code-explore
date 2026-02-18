using EnterpriseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for ProductVariant entity.
/// </summary>
public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.PriceAdjustment)
            .HasPrecision(18, 4);

        builder.Property(v => v.Barcode)
            .HasMaxLength(50);

        builder.Property(v => v.Weight)
            .HasPrecision(10, 3);

        builder.Property(v => v.ImageUrl)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(v => v.Sku)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(v => v.ProductId);

        builder.HasIndex(v => v.IsActive);

        builder.HasIndex(v => v.Barcode)
            .HasFilter("\"Barcode\" IS NOT NULL");

        // Global query filter for soft delete
        builder.HasQueryFilter(v => !v.IsDeleted);
    }
}
