using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface IAppSettingRepository
{
    Task<AppSetting> GetRetentionPeriod();
}