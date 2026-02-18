using EnterpriseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApp.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.TwoFactorSecret)
            .HasMaxLength(256);

        builder.Property(u => u.RefreshToken)
            .HasMaxLength(512);

        builder.Property(u => u.PasswordResetToken)
            .HasMaxLength(512);

        builder.Property(u => u.RowVersion)
            .IsRowVersion();

        // Audit fields
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.CreatedBy).IsRequired();

        // Soft delete
        builder.Property(u => u.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(u => !u.IsDeleted);

        // Index for search
        builder.HasIndex(u => new { u.LastName, u.FirstName });
        builder.HasIndex(u => u.Status);

        // Ignore computed property
        builder.Ignore(u => u.FullName);
    }
}
