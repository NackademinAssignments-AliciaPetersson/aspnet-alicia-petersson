namespace Domain.Exceptions.Custom;

public sealed class NotRemovedDomainException : DomainExceptionBase
{
    public NotRemovedDomainException(string message) : base(message) { }
    public NotRemovedDomainException(string message, Exception? innerException) : base(message, innerException) { }
}
