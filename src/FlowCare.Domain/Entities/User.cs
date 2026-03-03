using FlowCare.Domain.Enums;

namespace FlowCare.Domain.Entities;

public class User
{
    public string Id { get; private set; }
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public UserRole UserRole { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }

    // phone is nullable because some roles don't need it, only customer must have phone
    public string? Phone { get; private set; }

    // Branch ID is nullable because some roles don't need it, only branch manager and staff must have it
    public string? BranchId { get; private set; }
    public bool IsActive  { get; private set; }


    protected User()
    {
    }

    
    public User(string id, string userName, string password, UserRole userRole, string fullName, string email,
        string? phone,string? branchId ,bool isActive)
    {
        Id = id;
        UserName = userName;
        Password = password;
        UserRole = userRole;
        FullName = fullName;
        Email = email;
        IsActive = isActive;

        if (userRole == UserRole.CUSTOMER)
        {
            Phone = phone ?? throw new ArgumentException("Customer must have phone");
        }

        if (userRole == UserRole.BRANCH_MANAGER || userRole == UserRole.STAFF )
        {
            BranchId = branchId ?? throw new ArgumentException("Branch manager and staff must have branch Id");
        }
    }


}