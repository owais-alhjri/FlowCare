using System.Text.RegularExpressions;
using FlowCare.Application.Features.User.Customer.DTOs;
using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;


namespace FlowCare.Application.Interfaces.Services;

public class CustomerService(IPasswordHasher passwordHasher, ICustomerRepository customerRepository, IStorageService storageService) : ICustomerService
{

    public async Task<CustomerResponseDto> Register(CustomerRegisterDto userDto )
    {


        var isValidPassword = Regex.IsMatch(
            userDto.Password,
            @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[#?!@$%^&*-]).{8,}$"
        );
        if (string.IsNullOrWhiteSpace(userDto.Password) || !isValidPassword)
            throw new ArgumentException("Invalid password");


        if (await customerRepository.ExistsEmailAsync(userDto.Email) != null)
        {
            throw new ArgumentException("Email already exist");
        } 
        
        if (await customerRepository.ExistsUsernameAsync(userDto.UserName) != null)
        {
            throw new ArgumentException("Username already exist");
        }

        var prefix = "usr_cust_";
        var lastUser = await customerRepository.FetchLastId();
        string fullId;
        if (lastUser is null)
        {
            fullId = prefix + "001";
        }
        else
        {
            var lastIdString = lastUser.Id.Substring(prefix.Length).TrimStart('_'); // ← TrimStart
            var lastNumber = int.Parse(lastIdString);
            fullId = prefix + (lastNumber + 1).ToString("D3");
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(userDto.IdImage.FileName)}";
        using var stream = userDto.IdImage.OpenReadStream();
        string dbReference = await storageService.UploadFileAsync(
            "customer-ids",
            fileName,
            stream,
            userDto.IdImage.ContentType
        );


        var hash = passwordHasher.Hash(userDto.Password);
        var user = new User(fullId, userDto.UserName, hash, UserRole.CUSTOMER,
            userDto.FullName, userDto.Email, userDto.Phone,null, isActive:true);

        user.SetIdImagePath(dbReference);
        await customerRepository.Register(user);
        await customerRepository.SaveChangesAsync();

        return new CustomerResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            IdImagePath = user.IdImagePath,
            UserName = user.UserName,
            UserRole = user.UserRole,
            IsActive = user.IsActive,
            Phone = user.Phone,
        };

    }

    public async Task<User?> Login(string identifier, string password)
    {
        var user = await customerRepository.ExistsUsernameAsync(identifier) ?? await customerRepository.ExistsEmailAsync(identifier);

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

    public async Task<List<CustomerResponseDto>> CustomerList()
    {
        var customers = await customerRepository.ListTheCustomers()?? throw new ArgumentException("Customer list is not available");

        return customers.Select(c => new CustomerResponseDto
        {
            Id = c.Id,
            Email = c.Email,
            FullName = c.FullName,
            Phone = c.Phone,
            UserName = c.UserName,
            UserRole = c.UserRole,
            IsActive = c.IsActive
        }).ToList();
    }

    public async Task<CustomerResponseDto> GetCustomerById(string customerId)
    {
        var customer = await customerRepository.ExistIdAsync(customerId) ?? throw new ArgumentException("Customer is not found");
        return new CustomerResponseDto
        {
            Id = customer.Id,
            Email = customer.Email,
            FullName = customer.FullName,
            Phone = customer.Phone,
            UserName = customer.UserName,
            UserRole = customer.UserRole,
            IsActive = customer.IsActive
        };
    }
}