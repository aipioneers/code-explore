using EnterpriseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for InventoryCount entity.
/// </summary>
public class InventoryCountConfiguration : IEntityTypeConfiguration<InventoryCount>
{
    public void Configure(EntityTypeBuilder<InventoryCount> builder)
    {
        builder.ToTable("InventoryCounts");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.CountNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.CountType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.Notes)
            .HasMaxLength(2000);

        builder.HasMany(i => i.Items)
            .WithOne(item => item.InventoryCount)
            .HasForeignKey(item => item.InventoryCountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(i => i.CountNumber).IsUnique();
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.StartedAt);
    }
}

/// <summary>
/// EF Core configuration for InventoryCountItem entity.
/// </summary>
public class InventoryCountItemConfiguration : IEntityTypeConfiguration<InventoryCountItem>
{
    public void Configure(EntityTypeBuilder<InventoryCountItem> builder)
    {
        builder.ToTable("InventoryCountItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Notes)
            .HasMaxLength(500);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(i => i.InventoryCountId);
        builder.HasIndex(i => i.ProductId);
    }
}
