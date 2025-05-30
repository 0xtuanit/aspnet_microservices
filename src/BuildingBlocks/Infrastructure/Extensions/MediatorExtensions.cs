using Contracts.Common.Events;
using MediatR;
using Serilog;

namespace Infrastructure.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventAsync(
        this IMediator mediator,
        List<BaseEvent> domainEvents,
        ILogger logger)
    {
        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
            // var data = new SerializeService().Serialize(domainEvent);
            // logger.Information($"\n-----\nDomain event has been published!\n" +
            //                    $"Event: {domainEvent.GetType().Name}\n" +
            //                    $"Data: {data}\n-----\n");
        }
    }
}