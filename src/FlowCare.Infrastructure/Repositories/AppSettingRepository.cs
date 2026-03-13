using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;
using FlowCare.Infrastructure.Persistence;

namespace FlowCare.Infrastructure.Repositories;

public class AppSettingRepository(FlowCareDbContext dbContext) : IAppSettingRepository
{
    public async Task<AppSetting> GetRetentionPeriod()
    {
        var period = await dbContext.AppSettings.FindAsync("SlotRetentionDays") 
                     ?? throw new KeyNotFoundException("Key not found");

        return period;
    }
}