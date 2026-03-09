using FlowCare.Application.Features.User.Customer.DTOs;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface ICustomerRepository
{
    Task Register(User user);
    Task SaveChangesAsync();
    Task<User?> ExistsEmailAsync(string email);
    Task<User?> ExistsUsernameAsync(string username);
    Task<User?> ExistIdAsync(string id);
    Task<User?> ExistsByStaffId(string staffId);
    Task<User?> FetchLastId();
    Task<List<User>> ListTheCustomers();


}