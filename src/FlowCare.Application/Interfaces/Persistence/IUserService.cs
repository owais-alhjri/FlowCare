using FlowCare.Application.Features.User.DTOs;
using FlowCare.Domain.Entities;

namespace FlowCare.Application.Interfaces.Persistence;

public interface IUserService
{
    Task<User> Register(CustomerRegisterDto userRegisterDto );
    Task<User?> Login(string identifier, string password);
}