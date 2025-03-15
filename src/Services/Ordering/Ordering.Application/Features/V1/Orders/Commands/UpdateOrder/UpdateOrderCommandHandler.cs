using AutoMapper;
using MediatR;
using Ordering.Application.Common.Exceptions;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
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
        var orderEntity = await _repository.GetByIdAsync(command.Id);
        if (orderEntity is null) throw new NotFoundException(nameof(Order), command.Id);

        _logger.Information($"BEGIN: {MethodName} - Order: {command.Id}");

        orderEntity = _mapper.Map(command, orderEntity);
        var updatedOrder = await _repository.UpdateOrderAsync(orderEntity);
        _repository.SaveChangesAsync();

        _logger.Information($"Order {command.Id} was successfully updated.");
        var result = _mapper.Map<OrderDto>(updatedOrder);

        _logger.Information($"END: {MethodName} - Order: {command.Id}");

        return new ApiSuccessResult<OrderDto>(result);
    }
}