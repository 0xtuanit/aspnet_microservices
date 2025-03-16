using Contracts.Common.Events;
using Contracts.Domains.Interfaces;

namespace Contracts.Common.Interfaces;

public interface IEventEntity
{
    void AddDomainEvent(BaseEvent domainEvent);
    void RemoveDomainEvent(BaseEvent domainEvent);
    void ClearDomainEvents();
    IReadOnlyCollection<BaseEvent> DomainEvents();
}

// IEventEntity for doing DDD structure
public interface IEventEntity<T> : IEntityBase<T>, IEventEntity
{
}