using Contracts.Common.Interfaces;
using Contracts.Domains;

namespace Contracts.Common.Events;

// Methods used to apply DDD to the specific services
public class EventEntity<T> : EntityBase<T>, IEventEntity<T>
{
    private readonly List<BaseEvent> _domainEvents = new();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public IReadOnlyCollection<BaseEvent> DomainEvents() => _domainEvents.AsReadOnly();
}