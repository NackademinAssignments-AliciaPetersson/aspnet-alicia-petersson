namespace Domain.Exceptions.Custom;

public sealed class NotFoundDomainException : DomainExceptionBase
{
    public NotFoundDomainException(string message) : base(message) { }
    public NotFoundDomainException(string message, Exception? innerException) : base(message, innerException) { }
}
