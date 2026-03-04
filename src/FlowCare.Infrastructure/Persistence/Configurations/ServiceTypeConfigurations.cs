using FlowCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCare.Infrastructure.Persistence.Configurations;

public class ServiceTypeConfigurations : IEntityTypeConfiguration<ServiceType>
{
    public void Configure(EntityTypeBuilder<ServiceType> builder)
    {
        builder.ToTable("ServiceTypes");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(s => s.BranchId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(s => s.DurationMinutes)
            .IsRequired();
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(s => s.BranchId);

    }
}