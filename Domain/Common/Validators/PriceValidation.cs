using Domain.Exceptions.Custom;

namespace Domain.Common.Validators;

public static class PriceValidation
{
    public static decimal IsNotNegative(decimal value, string propertyName)
    {
        if (value < 0)
            throw new ValidationDomainException($"{propertyName} cannot be less than 0");
        return value;
    }
}
