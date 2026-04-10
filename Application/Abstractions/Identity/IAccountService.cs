using Application.Common.Outputs;
using Application.Common.Results;
using Application.Modules.Members.Inputs;
using Application.Modules.Members.Outputs;

namespace Application.Abstractions.Identity;

public interface IAccountService
{
    Task<Result> DeleteAuthenticationUserAsync(string userId);
    Task<Result<AuthenticationUserDetails?>> GetAuthenticationUserDetailsAsync(string userId);
    Task<Result> UpdateAuthenticationUserDetailsAsync(UpdateAuthenticationUserDetailsInput details);
}
