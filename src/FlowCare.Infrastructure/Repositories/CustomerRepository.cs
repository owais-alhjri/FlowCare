using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
using FlowCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Repositories;

public class CustomerRepository(FlowCareDbContext flowCareDb) : ICustomerRepository
{

    public async Task Register(User user)
    {
         await flowCareDb.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await flowCareDb.SaveChangesAsync();
    }

    public async Task<User?> ExistIdAsync(string id)
    {
        return await flowCareDb.Users.FindAsync(id);

    }

    public async Task<User?> ExistsEmailAsync(string email)
    {
        return await flowCareDb.Users.FirstOrDefaultAsync(x=>x.Email == email);
    }
    public async Task<User?> ExistsUsernameAsync(string username)
    { 
        return await flowCareDb.Users.FirstOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<User?> FetchLastId()
    {
          return await flowCareDb.Users.OrderByDescending(x=>x.Id).FirstOrDefaultAsync();
    }

}