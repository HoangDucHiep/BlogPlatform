namespace Lumo.Domain.Abstractions;

public abstract class Entity
{
    public Guid Id { get; init; }
    private readonly List<IDomainEvent> _domainEvents = new();


    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastUpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;

    protected Entity(Guid id)
    {
        Id = id;
        LastUpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    protected Entity()
    {
    }


    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
