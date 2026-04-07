using Domain.Exceptions.Custom;

namespace Domain.Common.Validators;

public static class StringValidation
{
    public static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationDomainException($"{propertyName} must be provided");

        return value.Trim();
    }
}
