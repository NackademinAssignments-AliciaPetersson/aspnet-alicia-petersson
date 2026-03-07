namespace Domain.Exceptions.Custom;

public sealed class ValidationDomainException : DomainExceptionBase
{
    public ValidationDomainException(string message) : base(message) { }
    public ValidationDomainException(string message, Exception? innerException) : base(message, innerException) { }
}