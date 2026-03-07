using System.Text.RegularExpressions;
using FlowCare.Application.Features.User.Customer.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;


namespace FlowCare.Application.Interfaces.Services;

public class CustomerService(IPasswordHasher passwordHasher, ICustomerRepository userRepository) : ICustomerService
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

        var customerIdPiss = "user_cust_";
        var lastUser = await userRepository.FetchLastId();

        string fullId;
        if (lastUser is null)
        {
            fullId = customerIdPiss + "001";
        }
        else
        {
            var lasIdString = lastUser.Id.Substring(customerIdPiss.Length);

            var lastNumber = int.Parse(lasIdString);
            var nextNumber = lastNumber + 1;

            fullId = customerIdPiss + nextNumber.ToString("D3");
        }

        var hash = passwordHasher.Hash(userDto.Password);
        var user = new User(fullId, userDto.UserName, hash, UserRole.CUSTOMER,
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