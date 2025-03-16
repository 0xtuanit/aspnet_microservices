using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Shared.SeedWork;
using ILogger = Serilog.ILogger;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, ApiResult<List<OrderDto>>>
{
    private readonly IMapper _mapper;
    private readonly IOrderRepository _repository;
    private readonly ILogger _logger;

    public GetOrdersQueryHandler(IMapper mapper, IOrderRepository repository, ILogger logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private const string MethodName = "GetOrdersQueryHandler";

    public async Task<ApiResult<List<OrderDto>?>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {MethodName} - Username: {request.Username}");

        var orderEntities = await _repository.GetOrdersByUsernameAsync(request.Username);
        var orderList = _mapper.Map<List<OrderDto>>(orderEntities);

        _logger.Information($"END: {MethodName} - Username: {request.Username}");

        return new ApiSuccessResult<List<OrderDto>?>(orderList);
    }
}