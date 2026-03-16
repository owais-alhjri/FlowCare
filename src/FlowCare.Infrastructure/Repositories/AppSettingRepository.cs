using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;
using FlowCare.Infrastructure.Persistence;

namespace FlowCare.Infrastructure.Repositories;

public class AppSettingRepository(FlowCareDbContext dbContext) : IAppSettingRepository
{
    public async Task<AppSetting> GetRetentionPeriod()
    {
         return await dbContext.AppSettings.FindAsync("SlotRetentionDays")
                     ?? throw new KeyNotFoundException("SlotRetentionDays not found");

    }

    public async Task<AppSetting> GetBookingLimitPerDay()
    {
        return await dbContext.AppSettings.FindAsync("LimitOfBookingPerDay")
               ?? throw new KeyNotFoundException("LimitOfBookingPerDay not found");
    }

    public async Task<AppSetting> GetReschedulingLimitPerDay()
    {
        return await dbContext.AppSettings.FindAsync("ReschedulingLimit")
               ?? throw new KeyNotFoundException("ReschedulingLimit not found");
    }
}