namespace Domain.Abstractions.Persistence;

public interface IEntity<TId>
{
    TId Id { get; set; }
}
