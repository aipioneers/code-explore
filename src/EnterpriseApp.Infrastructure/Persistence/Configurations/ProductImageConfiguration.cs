using EnterpriseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for ProductImage entity.
/// </summary>
public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(i => i.AltText)
            .HasMaxLength(200);

        builder.Property(i => i.Title)
            .HasMaxLength(200);

        // Indexes
        builder.HasIndex(i => i.ProductId);

        builder.HasIndex(i => i.DisplayOrder);

        builder.HasIndex(i => i.IsPrimary);

        // Global query filter for soft delete
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}
