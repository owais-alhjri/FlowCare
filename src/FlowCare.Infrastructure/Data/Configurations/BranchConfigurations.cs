using FlowCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCare.Infrastructure.Data.Configurations;

public class BranchConfigurations : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(b => b.City)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(b => b.Address)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(b => b.Timezone)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(b => b.IsActive)
            .IsRequired();
        builder.HasIndex(b => b.City)
            .IsUnique();
    }
}