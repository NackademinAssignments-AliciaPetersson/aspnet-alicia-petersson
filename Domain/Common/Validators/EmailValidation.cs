using Domain.Exceptions.Custom;
using System.Text.RegularExpressions;

namespace Domain.Common.Validators;

public static class EmailValidation
{
    public static string Validate(string email, string propertyName)
    {
        var trimmed = StringValidation.Required(email, propertyName);

        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        if (!Regex.IsMatch(trimmed, pattern))
        {
            throw new ValidationDomainException("Invalid email format");
        }

        return trimmed;
    }
}
