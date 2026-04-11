using Domain.Common.Validators;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;

namespace Infrastructure.Identity;

public class AuthenticationUser : IdentityUser
{
    private AuthenticationUser()
    {
        
    }
    public static AuthenticationUser Create(string email)
    {
        return new AuthenticationUser
        {
            UserName = EmailValidation.Validate(email, "Email"),
            Email = EmailValidation.Validate(email, "Email")
        };
    }
}
