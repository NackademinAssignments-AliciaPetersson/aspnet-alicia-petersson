using Application.Common.Results;

namespace Application.Abstractions.Identity;

public interface IAuthService
{
    Task<bool> DoesUserExistAsync(string email);
    Task<Result<string?>> SignUpLocalUserAsync(string email, string password, string? roleName = null);
}
