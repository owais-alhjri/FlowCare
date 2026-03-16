using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces;

public interface IAppSettingRepository
{
    Task<AppSetting> GetRetentionPeriod();
    Task<AppSetting> GetBookingLimitPerDay();
}