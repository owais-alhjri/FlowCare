using FlowCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCare.Infrastructure.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(u => u.UserRole)
            .IsRequired()
            .HasConversion<string>();
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(u => u.Phone)
            .HasMaxLength(12);
        builder.Property(u => u.BranchId)
            .HasMaxLength(100);
        builder.Property(u => u.IsActive)
            .IsRequired();
        builder.Property(u => u.IdImagePath)
            .HasMaxLength(250);
        builder.HasIndex(u => u.Email)
            .IsUnique();
        builder.HasIndex(u => u.UserName)
            .IsUnique();
    }
}