using System.Text.RegularExpressions;
using FlowCare.Application.Common;
using FlowCare.Application.Features.User.Customer.DTOs;
using FlowCare.Application.Interfaces;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;


namespace FlowCare.Application.Services;

public class CustomerService(
    IPasswordHasher passwordHasher,
    ICustomerRepository customerRepository,
    IStorageService storageService) : ICustomerService
{
    public async Task<Result<CustomerResponseDto>> Register(CustomerRegisterDto userDto)
    {
        var isValidPassword = Regex.IsMatch(
            userDto.Password,
            @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[#?!@$%^&*-]).{8,}$"
        );
        if (string.IsNullOrWhiteSpace(userDto.Password) || !isValidPassword)
            return Result<CustomerResponseDto>.Fail("Invalid password");


        if (await customerRepository.FindByEmailAsync(userDto.Email) != null)
        {
            return Result<CustomerResponseDto>.Fail("Email already exist");
        }

        if (await customerRepository.FindByUsernameAsync(userDto.UserName) != null)
        {
            return Result<CustomerResponseDto>.Fail("Username already exist");
        }

        var prefix = "usr_cust_";
        var lastUser = await customerRepository.GetLastId();
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
            userDto.FullName, userDto.Email, userDto.Phone, null, isActive: true);

        user.SetIdImagePath(dbReference);
        await customerRepository.Register(user);
        await customerRepository.SaveChangesAsync();

        return Result<CustomerResponseDto>.Success(new CustomerResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            IdImagePath = user.IdImagePath,
            UserName = user.UserName,
            UserRole = user.UserRole,
            IsActive = user.IsActive,
            Phone = user.Phone,
        });
    }

    public async Task<Result<User>> Login(string identifier, string password)
    {
        // it supports username or email
        var user = await customerRepository.FindByUsernameAsync(identifier)
                   ?? await customerRepository.FindByEmailAsync(identifier);

        if (user is null)
            return Result<User>.Fail("Invalid credentials");

        if (!passwordHasher.Verify(password, user.Password))
            return Result<User>.Fail("Invalid credentials");

        return Result<User>.Success(user);
    }

    public async Task<Result<List<CustomerResponseDto>>> CustomerList()
    {
        var customers = await customerRepository.ListTheCustomers();
        if (customers is null)
            return Result<List<CustomerResponseDto>>.Fail("Customer list is not available");

        var result = customers.Select(c => new CustomerResponseDto
        {
            Id = c.Id,
            Email = c.Email,
            FullName = c.FullName,
            Phone = c.Phone,
            UserName = c.UserName,
            UserRole = c.UserRole,
            IsActive = c.IsActive
        }).ToList();

        return Result<List<CustomerResponseDto>>.Success(result);
    }

    public async Task<Result<CustomerResponseDto>> GetCustomerById(string customerId)
    {
        var customer = await customerRepository.FindByIdAsync(customerId);
        if (customer is null)
            return Result<CustomerResponseDto>.Fail("Customer not found");

        return Result<CustomerResponseDto>.Success(new CustomerResponseDto
        {
            Id = customer.Id,
            Email = customer.Email,
            FullName = customer.FullName,
            Phone = customer.Phone,
            UserName = customer.UserName,
            UserRole = customer.UserRole,
            IsActive = customer.IsActive,
            IdImagePath = customer.IdImagePath
        });
    }
}