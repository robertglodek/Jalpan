namespace Taskly.Services.Task.Domain.Entities;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IEnumerable<IDomainEvent> DomainEvents => _domainEvents;

    public AggregateId Id { get; protected set; } = null!;

    public int Version { get; protected set; }

    public DateTime CreatedAt { get; set; }

    protected void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

    public void RemoveDomainEvent(IDomainEvent @event) => _domainEvents.Remove(@event);

    public void ClearDomainEvents() => _domainEvents.Clear();
}