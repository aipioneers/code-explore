using EnterpriseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Product entity.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(p => p.ShortDescription)
            .HasMaxLength(500);

        builder.Property(p => p.Description)
            .HasMaxLength(10000);

        builder.Property(p => p.BasePrice)
            .HasPrecision(18, 4);

        builder.Property(p => p.CostPrice)
            .HasPrecision(18, 4);

        builder.Property(p => p.TaxRate)
            .HasPrecision(5, 2);

        builder.Property(p => p.Weight)
            .HasPrecision(10, 3);

        builder.Property(p => p.Dimensions)
            .HasMaxLength(100);

        builder.Property(p => p.Manufacturer)
            .HasMaxLength(100);

        builder.Property(p => p.ManufacturerPartNumber)
            .HasMaxLength(100);

        builder.Property(p => p.Barcode)
            .HasMaxLength(50);

        builder.Property(p => p.MetaTitle)
            .HasMaxLength(100);

        builder.Property(p => p.MetaDescription)
            .HasMaxLength(300);

        builder.Property(p => p.MetaKeywords)
            .HasMaxLength(300);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.Sku)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(p => p.Slug)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(p => p.CategoryId);

        builder.HasIndex(p => p.Status);

        builder.HasIndex(p => p.Name);

        builder.HasIndex(p => p.Barcode)
            .HasFilter("\"Barcode\" IS NOT NULL");

        // Full-text search index
        builder.HasIndex(p => new { p.Name, p.Sku })
            .HasDatabaseName("IX_Products_Search");

        // Global query filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
