using FlowCare.Application.Features.User.Customer.DTOs;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponseDto> Register(CustomerRegisterDto userRegisterDto );
    Task<User?> Login(string identifier, string password);
    Task<List<CustomerResponseDto>> CustomerList();
    Task<CustomerResponseDto> GetCustomerById(string customerId);
}