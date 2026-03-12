using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Application.Interfaces.Services;
using FlowCare.Infrastructure.Persistence;
using FlowCare.Infrastructure.Repositories;
using FlowCare.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlowCare.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<BranchesService>();
        services.AddScoped<IBranchesRepository, BranchesRepository>();
        services.AddScoped<IServicesTypeRepository, ServicesTypeRepository>();
        services.AddScoped<ServiceTypeService>();
        services.AddScoped<ISlotsRepository, SlotRepository>();
        services.AddScoped<SlotService>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<AppointmentService>();
        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<StaffService>();
        services.AddScoped<IStaffServiceTypeRepository, StaffServiceTypeRepository>();
        services.AddScoped<StaffServiceService>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<AuditLogService>();
        services.AddScoped<IAppSettingRepository, AppSettingRepository>();
        services.AddScoped<DataSeeder>();
        services.AddSingleton<FileValidationService>();
        services.AddScoped<IStorageService, MinioStorageService>();


        return services;
    }


}