using MediatR;
using Ordering.Application.Common.Exceptions;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using ILogger = Serilog.ILogger;

namespace Ordering.Application.Features.V1.Orders;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger _logger;

    public DeleteOrderCommandHandler(IOrderRepository repository, ILogger logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private const string MethodName = "DeleteOrderCommandHandler";

    public async Task Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
    {
        var orderEntity = await _repository.GetByIdAsync(command.Id);
        if (orderEntity == null) throw new NotFoundException(nameof(Order), command.Id);

        _repository.DeleteAsync(orderEntity);
        _repository.SaveChangesAsync();

        _logger.Information($"Order {orderEntity.Id} was successfully deleted.");
    }
}