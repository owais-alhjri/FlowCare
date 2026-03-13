using FlowCare.Application.Common;
using FlowCare.Application.Features.User.Customer.DTOs;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces;

public interface ICustomerService
{
    Task<Result<CustomerResponseDto>> Register(CustomerRegisterDto userRegisterDto);
    Task<Result<User>> Login(string identifier, string password);
    Task<Result<List<CustomerResponseDto>>> CustomerList();
    Task<Result<CustomerResponseDto>> GetCustomerById(string customerId);
}