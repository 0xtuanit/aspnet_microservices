using Contracts.Common.Interfaces;
using Infrastructure.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventAsync(this IMediator mediator, DbContext context, ILogger logger)
    {
        var domainEntities = context.ChangeTracker.Entries<IEventEntity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents().Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents())
            .ToList();

        domainEntities.ForEach(x => x.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
            var data = new SerializeService().Serialize(domainEvent);
            logger.Information($"\n-----\nDomain event {domainEvent.GetType().Name} has been published!\n" +
                               $"Data: {data}\n-----\n");
        }
    }
}