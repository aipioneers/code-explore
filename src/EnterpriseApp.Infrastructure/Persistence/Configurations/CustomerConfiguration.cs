using EnterpriseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApp.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CustomerNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.CustomerNumber)
            .IsUnique();

        builder.Property(c => c.CompanyName)
            .HasMaxLength(200);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(c => c.Email);

        builder.Property(c => c.Phone)
            .HasMaxLength(30);

        builder.Property(c => c.Mobile)
            .HasMaxLength(30);

        builder.Property(c => c.Fax)
            .HasMaxLength(30);

        builder.Property(c => c.Website)
            .HasMaxLength(500);

        builder.Property(c => c.TaxId)
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

        builder.Property(c => c.CreditLimit)
            .HasPrecision(18, 2);

        builder.Property(c => c.PaymentTermsDays)
            .IsRequired()
            .HasDefaultValue(30);

        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        // Audit fields
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.CreatedBy).IsRequired();

        // Soft delete
        builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(c => !c.IsDeleted);

        // Relationships
        builder.HasMany(c => c.Addresses)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore computed properties
        builder.Ignore(c => c.DisplayName);
        builder.Ignore(c => c.FullName);

        // Indexes for full-text search
        builder.HasIndex(c => new { c.LastName, c.FirstName });
        builder.HasIndex(c => c.CompanyName);

        // PostgreSQL full-text search
        builder.HasIndex(c => c.CustomerNumber)
            .HasDatabaseName("IX_Customers_CustomerNumber_FullText");
    }
}
