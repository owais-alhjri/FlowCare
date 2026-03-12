using System.Net.Mail;
using System.Text.RegularExpressions;
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
    public string? IdImagePath { get; private set; }

    protected User()
    {
    }

    public User(string id, string userName, string password, UserRole userRole, string fullName, string email,
        string? phone,string? branchId ,bool isActive)
    {
        ValidateCommon(id, userName, password, fullName, email);
        ValidateRole(phone, userRole, branchId);

        Id = id;
        UserName = userName;
        Password = password;
        UserRole = userRole;
        FullName = fullName;
        Email = email;
        IsActive = isActive;
    }

    public void SetIdImagePath(string path)
    {
        if (UserRole != UserRole.CUSTOMER)
            throw new InvalidOperationException("Only customers can have an ID image.");

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("ID image path cannot be empty.");

        IdImagePath = path;
    }

    private void ValidateRole(string? phone, UserRole userRole, string? branchId)
    {

        // if the manager or staff entered a phone, he will get error
        if (userRole != UserRole.CUSTOMER && phone is not null)
            throw new ArgumentException("Only customers can have phone numbers");

        if (userRole == UserRole.BRANCH_MANAGER || userRole == UserRole.STAFF)
        {
            if (string.IsNullOrWhiteSpace(branchId) || branchId.Length < 6 || branchId.Length > 100)
            {
                throw new ArgumentException("Invalid branch ID");
            }

            BranchId = branchId;
        }
        // if customer entered a branch ID it will throw error
        if ((userRole != UserRole.BRANCH_MANAGER && userRole != UserRole.STAFF) && branchId is not null)
        {
            throw new ArgumentException("Only branch manager and staff can have branch ID");
        }
        var isValidPhone = Regex.IsMatch(phone ?? "", @"^(\+968)?\d{8}$");
        if (userRole == UserRole.CUSTOMER)
        {
            if (!isValidPhone)
                throw new ArgumentException("Invalid Oman phone number");
            Phone = phone!.StartsWith("+968") ? phone : $"+968{phone}";

        }

    }

    private static void ValidateCommon(string id, string userName, string password, string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Invalid ID");
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Invalid username");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Invalid password");
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Invalid full name");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Invalid email");

        var isValidPassword = Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[#?!@$%^&*-]).{8,}$");
        var isValidEmail = MailAddress.TryCreate(email, out _);

        if (id.Length < 6 || id.Length > 100) throw new ArgumentException("Invalid ID");
        if (!isValidPassword) throw new ArgumentException("Invalid password");
        if (userName.Length < 3 || userName.Length > 100) throw new ArgumentException("Invalid username");
        if (fullName.Length < 6 || fullName.Length > 100) throw new ArgumentException("Invalid full name");
        if (!isValidEmail) throw new ArgumentException("Invalid email");

    }

}