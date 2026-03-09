using FlowCare.Application.Features.User.Customer.DTOs;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface ICustomerService
{
    Task<User> Register(CustomerRegisterDto userRegisterDto );
    Task<User?> Login(string identifier, string password);
    Task<List<CustomerResponseDto>> CustomerList();
    Task<CustomerResponseDto> GetCustomerById(string customerId);
}