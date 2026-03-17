using System.Text.Json;
using System.Text.Json.Serialization;
using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;
using FlowCare.Infrastructure.Data.SeedDtos;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Data;

public class DataSeeder(FlowCareDbContext db, IPasswordHasher passwordHasher)
{
    public async Task SeedAsync()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var jsonPath = Path.Combine(basePath, "example.json");

        if (!File.Exists(jsonPath))
            throw new FileNotFoundException($"Seed file not found at: {jsonPath}");

        var json = await File.ReadAllTextAsync(jsonPath);
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
        var seedData = JsonSerializer.Deserialize<SeedData>(json, options)
                       ?? throw new Exception("Failed to deserialize seed data.");

        var allUserDtos = (seedData.Users.Admin ?? [])
            .Concat(seedData.Users.BranchManagers ?? [])
            .Concat(seedData.Users.Staff ?? [])
            .Concat(seedData.Users.Customers ?? [])
            .ToList();


        await SeedEntitiesAsync(db.Users, allUserDtos,
            u => new User(u.Id, u.UserName, passwordHasher.Hash(u.Password), u.UserRole, u.FullName, u.Email, u.Phone,
                u.BranchId, u.IsActive));

        await SeedEntitiesAsync(db.Branches, seedData.Branches,
            b => new Branch(b.Id, b.Name, b.City, b.Address, b.Timezone, b.IsActive));

        await SeedEntitiesAsync(db.ServiceTypes, seedData.ServiceTypes,
            s => new ServiceType(s.Id, s.BranchId, s.Name, s.Description, s.DurationMinutes, s.IsActive));

        await db.SaveChangesAsync();
        var seededUsers = await db.Users.ToDictionaryAsync(u => u.Id);

        await SeedEntitiesAsync(db.StaffServiceTypes, seedData.StaffServiceTypes,
            s => new StaffServiceType(seededUsers[s.StaffId], s.ServiceTypeId),
            keySelector: s => s.StaffId + "_" + s.ServiceTypeId,
            existingKeySelector: e => e.StaffId + "_" + e.ServiceTypeId);

        await SeedEntitiesAsync(db.Slots, seedData.Slots,
            s => new Slot(s.Id, s.BranchId, s.ServiceTypeId, seededUsers[s.StaffId],
                s.StartAt.ToUniversalTime(), (int)(s.EndAt - s.StartAt).TotalMinutes, s.Capacity, s.IsActive));

        await SeedEntitiesAsync(db.Appointments, seedData.Appointments,
            a => new Appointment(a.Id, seededUsers[a.CustomerId], seededUsers[a.StaffId],
                a.BranchId, a.ServiceTypeId, a.SlotId, a.Status, a.CreatedAt.ToUniversalTime(), a.Queue));

        var existingAuditIds = (await db.AuditLogs.Select(a => a.Id).ToListAsync()).ToHashSet();
        var newAuditLogs = seedData.AuditLogs?
            .Where(a => !existingAuditIds.Contains(a.Id))
            .ToList();
        if (newAuditLogs?.Count > 0)
            await db.AuditLogs.AddRangeAsync(newAuditLogs.Select(a =>
                CreateAuditLogDirectly(a)));
        await SeedEntitiesAsync(db.AppSettings, seedData.SystemSettings,
            s => new AppSetting(s.Key, s.Value, s.Description),
            keySelector: s => s.Key,
            existingKeySelector: e => e.Key);

        await db.SaveChangesAsync();

        Console.WriteLine("✅ Seed complete.");
    }

    private static AuditLog CreateAuditLogDirectly(AuditLogSeedDto dto)
    {
        var auditLog = (AuditLog)Activator.CreateInstance(typeof(AuditLog), nonPublic: true)!;

        typeof(AuditLog).GetProperty(nameof(AuditLog.Id))!
            .SetValue(auditLog, dto.Id);
        typeof(AuditLog).GetProperty(nameof(AuditLog.ActorId))!
            .SetValue(auditLog, dto.ActorId);
        typeof(AuditLog).GetProperty(nameof(AuditLog.ActorRole))!
            .SetValue(auditLog, dto.ActorRole);
        typeof(AuditLog).GetProperty(nameof(AuditLog.ActionType))!
            .SetValue(auditLog, dto.ActionType);
        typeof(AuditLog).GetProperty(nameof(AuditLog.EntityType))!
            .SetValue(auditLog, dto.EntityType);
        typeof(AuditLog).GetProperty(nameof(AuditLog.EntityId))!
            .SetValue(auditLog, dto.EntityId);
        typeof(AuditLog).GetProperty(nameof(AuditLog.Timestamp))!
            .SetValue(auditLog, dto.Timestamp.ToUniversalTime());
        typeof(AuditLog).GetProperty(nameof(AuditLog.Metadata))!
            .SetValue(auditLog, dto.Metadata);

        return auditLog;
    }

    private static async Task SeedEntitiesAsync<TDto, TEntity>(
        DbSet<TEntity> dbSet,
        List<TDto>? data,
        Func<TDto, TEntity> map,
        Func<TDto, string>? keySelector = null,
        Func<TEntity, string>? existingKeySelector = null)
        where TEntity : class
    {
        if (data == null || data.Count == 0) return;

        HashSet<string> existingIds;

        var query = dbSet.IgnoreQueryFilters();

        if (existingKeySelector != null)
        {
            existingIds = (await query.ToListAsync())
                .Select(existingKeySelector)
                .ToHashSet();
        }
        else
        {
            existingIds = (await query
                    .Select(e => EF.Property<string>(e, "Id"))
                    .ToListAsync())
                .ToHashSet();
        }

        var newEntries = data
            .Where(d =>
            {
                var key = keySelector != null
                    ? keySelector(d)
                    : (string)typeof(TDto).GetProperty("Id")!.GetValue(d)!;
                return !existingIds.Contains(key);
            })
            .Select(map)
            .ToList();

        if (newEntries.Count > 0)
            await dbSet.AddRangeAsync(newEntries);
    }

    private static DateTimeOffset ToUtc(DateTimeOffset dto) => dto.ToUniversalTime();
}