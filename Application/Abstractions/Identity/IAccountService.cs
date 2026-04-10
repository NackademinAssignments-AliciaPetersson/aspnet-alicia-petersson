using Application.Common.Results;

namespace Application.Abstractions.Identity;

public interface IAccountService
{
    Task<Result> RemoveMemberAsync(string userId);
}
