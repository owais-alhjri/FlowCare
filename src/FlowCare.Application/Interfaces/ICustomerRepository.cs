using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces;

public interface ICustomerRepository
{
    Task Register(User user);
    Task SaveChangesAsync();
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByUsernameAsync(string username);
    Task<User?> FindByIdAsync(string id);
    Task<User?> FindByStaffId(string staffId);
    Task<User?> GetLastId();
    Task<List<User>> ListTheCustomers();
}