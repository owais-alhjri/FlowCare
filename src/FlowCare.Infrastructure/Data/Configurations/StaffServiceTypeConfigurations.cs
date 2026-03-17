using FlowCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCare.Infrastructure.Data.Configurations;

public class StaffServiceTypeConfigurations : IEntityTypeConfiguration<StaffServiceType>
{
    public void Configure(EntityTypeBuilder<StaffServiceType> builder)
    {
        builder.ToTable("StaffServiceTypes");
        builder.HasKey(s => new { s.StaffId, s.ServiceTypeId });
        builder.Property(s => s.StaffId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(s => s.ServiceTypeId)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasOne(s => s.Staff)
            .WithMany()
            .HasForeignKey(s => s.StaffId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ServiceType>()
            .WithMany()
            .HasForeignKey(a => a.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}