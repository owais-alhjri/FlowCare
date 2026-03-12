using FlowCare.Application.Interfaces.Persistence;
using FlowCare.Domain.Entities;
using FlowCare.Domain.Enums;
using FlowCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowCare.Infrastructure.Repositories;

public class CustomerRepository(FlowCareDbContext dbContext) : ICustomerRepository
{

    public async Task Register(User user)
    {
         await dbContext.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task<User?> ExistIdAsync(string id)
    {
        return await dbContext.Users.FindAsync(id);

    }

    public async Task<User?> ExistsEmailAsync(string email)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x=>x.Email == email);
    }
    public async Task<User?> ExistsUsernameAsync(string username)
    { 
        return await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<User?> ExistsByStaffId(string staffId)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == staffId);
    }

    public async Task<User?> FetchLastId()
    {
        return await dbContext.Users
            .Where(x => x.Id.StartsWith("usr_cust_"))
            .OrderByDescending(x => x.Id.Length)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }


    public async Task<List<User>> ListTheCustomers()
    {

        return await dbContext.Users.AsNoTracking().Where(u => u.UserRole == UserRole.CUSTOMER).ToListAsync();

    }
}