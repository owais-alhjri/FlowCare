using FlowCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Persistence;

public class FlowCareDbContext(DbContextOptions<FlowCareDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AuditLog> AuditLogs  { get; set;}
    public DbSet<Branch> Branches  { get; set;}
    public DbSet<ServiceType> ServiceTypes  { get; set;}
    public DbSet<StaffServiceType> StaffServiceTypes  { get; set;}
    public DbSet<Slot> Slots  { get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowCareDbContext).Assembly);
    }
}