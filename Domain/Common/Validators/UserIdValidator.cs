using Domain.Exceptions.Custom;

namespace Domain.Common.Validators;

public static class UserIdValidator
{
    public static string EnsureValidGuid(string? id)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
        {
            throw new ValidationDomainException("Id not valid");
        }

        return id;
    }
}
