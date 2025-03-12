using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using ILogger = Serilog.ILogger;

namespace Ordering.Application.Features.V1.Orders;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IMapper _mapper;
    private readonly IOrderRepository _repository;
    private readonly ILogger _logger;

    public DeleteOrderCommandHandler(IMapper mapper, IOrderRepository repository, ILogger logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private const string MethodName = "DeleteOrderCommandHandler";

    public async Task Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {MethodName}");

        await _repository.DeleteOrder(command.Id);
        await _repository.SaveChangesAsync();

        _logger.Information($"END: {MethodName}");
    }
}