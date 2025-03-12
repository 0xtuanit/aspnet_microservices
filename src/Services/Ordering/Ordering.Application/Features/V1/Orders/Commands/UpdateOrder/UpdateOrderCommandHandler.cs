using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Shared.SeedWork;
using ILogger = Serilog.ILogger;

namespace Ordering.Application.Features.V1.Orders;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, ApiResult<OrderDto>>
{
    private readonly IMapper _mapper;
    private readonly IOrderRepository _repository;
    private readonly ILogger _logger;

    public UpdateOrderCommandHandler(IMapper mapper, IOrderRepository repository, ILogger logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private const string MethodName = "UpdateOrderCommandHandler";

    public async Task<ApiResult<OrderDto>> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {MethodName}");

        var oldOrder = await _repository.GetOrder(command.Id);
        if (oldOrder == null)
            return new ApiErrorResult<OrderDto>("Order not found with the provided id.");

        var updatedOrder = _mapper.Map(command, oldOrder);

        await _repository.UpdateOrder(updatedOrder);
        await _repository.SaveChangesAsync();

        var result = _mapper.Map<OrderDto>(updatedOrder);

        _logger.Information($"END: {MethodName}");

        return new ApiSuccessResult<OrderDto>(result);
    }
}