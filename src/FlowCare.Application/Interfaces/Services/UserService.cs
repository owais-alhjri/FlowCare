using System.Text.RegularExpressions;
using FlowCare.Application.Features.User.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;

namespace FlowCare.Application.Interfaces.Services;

public class UserService(IPasswordHasher passwordHasher, IUserRepository userRepository) : IUserService
{

    public async Task<User> Register(CustomerRegisterDto userDto )
    {
        var isValidPassword = Regex.IsMatch(
            userDto.Password,
            @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[#?!@$%^&*-]).{8,}$"
        );
        if (string.IsNullOrWhiteSpace(userDto.Password) || !isValidPassword)
            throw new ArgumentException("Invalid password");


        if (await userRepository.ExistsEmailAsync(userDto.Email) != null)
        {
            throw new ArgumentException("Email already exist");
        } 
        
        if (await userRepository.ExistsUsernameAsync(userDto.UserName) != null)
        {
            throw new ArgumentException("Username already exist");
        }

        var id = "";
        // TODO: I need to make the id add 1 automatic like usr_cust_001 the next one will be usr_cust_002
        var hash = passwordHasher.Hash(userDto.Password);
        var user = new User(id, userDto.UserName, hash, UserRole.CUSTOMER,
            userDto.FullName, userDto.Email, userDto.Phone,null, isActive:true);

        await userRepository.Register(user);
        await userRepository.SaveChangesAsync();

        return user;

    }

    public async Task<User?> Login(string identifier, string password)
    {
        var user = await userRepository.ExistsUsernameAsync(identifier) ?? await userRepository.ExistsEmailAsync(identifier);

        if (user == null )
        {
            return null;
        }

        if (!passwordHasher.Verify(password, user.Password))
        {
            return null;
        }

        return user;
    }
}