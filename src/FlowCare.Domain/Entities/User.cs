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

    private void ValidateRole(string? phone, UserRole userRole, string? branchId)
    {

        // if the manager or staff entered a phone, he will get error
        if (userRole != UserRole.CUSTOMER && phone is not null)
            throw new ArgumentException("Only customers can have phone numbers");

        if (userRole == UserRole.BRANCH_MANAGER || userRole == UserRole.STAFF)
        {
            if (string.IsNullOrWhiteSpace(branchId) || branchId.Length < 3 || branchId.Length > 100)
            {
                throw new ArgumentException("Invalid branch ID");
            }

            BranchId = branchId;
        }
        // if customer entered a branch ID it will throw error
        if ((userRole != UserRole.BRANCH_MANAGER && userRole != UserRole.STAFF) && branchId is not null)
        {
            throw new ArgumentException("Customer can not have branch ID");
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
        var isValidPassword = Regex.IsMatch(
            password,
            @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[#?!@$%^&*-]).{8,}$"
        );
        var isValidEmail = MailAddress.TryCreate(email, out _);

        if (string.IsNullOrWhiteSpace(id) || id.Length < 3 || id.Length > 100)
        {
            throw new ArgumentException("Invalid ID");
        }

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Invalid Password");
        if (!isValidPassword)
        {
            throw new ArgumentException("Invalid password");
        }

        if (string.IsNullOrWhiteSpace(userName) || userName.Length < 3 || userName.Length >100)
        {
            throw new ArgumentException("Invalid username");
        }

        if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 3 || fullName.Length > 100)
        {
            throw new ArgumentException("Invalid full name");
        }

        if (string.IsNullOrWhiteSpace(email) || !isValidEmail)
        {
            throw new ArgumentException("Invalid email");
        }

    }

}