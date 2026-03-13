using FlowCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCare.Infrastructure.Persistence.Configurations;

public class AppSettingConfigurations : IEntityTypeConfiguration<AppSetting>
{
    public void Configure(EntityTypeBuilder<AppSetting> builder)
    {
        builder.ToTable("AppSettings");
        builder.HasKey(a => a.Key);
        builder.Property(a => a.Key)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.Value)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(a => a.Description)
            .HasMaxLength(200);
    }
}