using Application.Common.Results;
using Application.Modules.Members.Inputs;

namespace Application.Abstractions.Identity;

public interface IAuthService
{
    Task<bool> DoesUserExistAsync(string email);
    Task<Result<string?>> SignUpLocalUserAsync(string email, string password, string? roleName = null);
    Task<Result> SignInLocalUserAsync(string email, string password, bool rememberMe = false);
    Task<Result<CreateExternalMemberInput?>> SignInExternalMemberAsync(string roleName = "Member");    
    Task SignOutUserAsync();
}
