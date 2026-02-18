using EnterpriseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Document entity.
/// </summary>
public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(d => d.TotalAmount)
            .HasPrecision(18, 4);

        builder.Property(d => d.Currency)
            .HasMaxLength(3);

        builder.Property(d => d.FilePath)
            .HasMaxLength(500);

        builder.Property(d => d.FileName)
            .HasMaxLength(255);

        builder.Property(d => d.MimeType)
            .HasMaxLength(100);

        builder.Property(d => d.Notes)
            .HasMaxLength(2000);

        // Relationships
        builder.HasOne(d => d.Order)
            .WithMany()
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.Customer)
            .WithMany()
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(d => d.DocumentNumber).IsUnique();
        builder.HasIndex(d => d.Type);
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.OrderId);
        builder.HasIndex(d => d.CustomerId);
        builder.HasIndex(d => d.DocumentDate);

        // Global query filter for soft delete
        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
