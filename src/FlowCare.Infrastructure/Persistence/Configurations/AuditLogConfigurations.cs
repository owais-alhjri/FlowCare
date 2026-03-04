using FlowCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCare.Infrastructure.Persistence.Configurations;

public class AuditLogConfigurations : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .IsRequired()
            .HasMaxLength(100);        
        builder.Property(a => a.ActionType)
            .IsRequired()
            .HasMaxLength(100);        
        builder.Property(a => a.ActorId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.ActorRole)
            .IsRequired();
        builder.Property(a => a.EntityId)
            .IsRequired()
            .HasMaxLength(100);        
        builder.Property(a => a.EntityType)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.Timestamp)
            .IsRequired();
        builder.Property(a => a.Metadata)
            .IsRequired();
        builder.HasIndex(a => a.ActorId);
    }
}