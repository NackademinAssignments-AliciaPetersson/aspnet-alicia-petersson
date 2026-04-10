namespace Domain.Exceptions.Custom;

public sealed class NotUpdatedDomainException : DomainExceptionBase
{
    public NotUpdatedDomainException(string message) : base(message) { }
    public NotUpdatedDomainException(string message, Exception? innerException) : base(message, innerException) { }
}
