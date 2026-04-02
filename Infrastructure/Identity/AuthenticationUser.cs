using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;

namespace Infrastructure.Identity;

public class AuthenticationUser : IdentityUser
{
    public static AuthenticationUser Create(string email)
    {
        return new AuthenticationUser
        {
            UserName = email,
            Email = email
        };
    }
}
