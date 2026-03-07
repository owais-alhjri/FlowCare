using FlowCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCare.Infrastructure.Persistence.Configurations;

public class AppointmentConfigurations: IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.CustomerId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.BranchId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.ServiceTypeId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.SlotId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.StaffId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>();
        builder.Property(a => a.CreatedAt)
            .IsRequired();
        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(a => a.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ServiceType>()
            .WithMany()
            .HasForeignKey(a => a.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Slot>()
            .WithMany()
            .HasForeignKey(a => a.SlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a=>a.Customer)
            .WithMany()
            .HasForeignKey(a =>a.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Staff)
            .WithMany()
            .HasForeignKey(a => a.StaffId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(a => new { a.SlotId, a.StaffId })
            .IsUnique();
        builder.HasIndex(a => a.CustomerId);
        builder.HasIndex(a => a.BranchId);
        builder.HasIndex(a => a.ServiceTypeId);

    }
}