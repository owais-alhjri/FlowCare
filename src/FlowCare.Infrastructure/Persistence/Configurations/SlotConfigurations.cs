using FlowCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCare.Infrastructure.Persistence.Configurations;

public class SlotConfigurations :IEntityTypeConfiguration<Slot>
{
    public void Configure(EntityTypeBuilder<Slot> builder)
    {
        builder.ToTable("Slots");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(s => s.BranchId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(s => s.ServiceTypeId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(s => s.StaffId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(s => s.Capacity)
            .IsRequired();
        builder.Property(a => a.StartedAt)
            .IsRequired();
        builder.Property(s => s.EndAt)
            .IsRequired();
        builder.Property(s => s.IsActive)
            .IsRequired();
        builder.HasOne(s => s.Staff)
            .WithMany()
            .HasForeignKey(s => s.StaffId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(s => new { s.BranchId, s.ServiceTypeId, s.StaffId });
        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ServiceType>()
            .WithMany()
            .HasForeignKey(s => s.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);




    }
}