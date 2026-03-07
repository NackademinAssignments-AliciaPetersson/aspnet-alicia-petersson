namespace Domain.Exceptions.Custom;

public sealed class NullDomainException : DomainExceptionBase
{
    public NullDomainException(string message) : base(message) { }
    public NullDomainException(string message, Exception? innerException) : base(message, innerException) { }
}
